using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Web.Routing;
using MvcSiteMapBuilder.Extensions;
using MvcSiteMapBuilder.Web.Mvc;

namespace MvcSiteMapBuilder.Security
{
    public class AuthorizeAttributeAclModule : IAclModule
    {
        public bool IsAccessibleToUser(SiteMapNode siteMapNode)
        {
            // not clickable? always accessible.
            if (!siteMapNode.Clickable)
                return true;

            var httpContext = new HttpContextWrapper(HttpContext.Current);

            // is it an external url?
            if (siteMapNode.HasExternalUrl(httpContext))
                return true;

            return VerifyNode(siteMapNode, httpContext);
        }

        #region Protected Members

        protected virtual bool VerifyNode(SiteMapNode node, HttpContextBase httpContext)
        {
            var routes = this.FindRoutesForNode(node, httpContext);
            if (routes == null)
                return true; // static URLs will sometimes have no route data, therefore return true.

            // time to delve into the AuthorizeAttribute defined on the node.
            // let's start by getting all metadata for the controller...
            var controllerType = ControllerTypeResolver.ResolveControllerType(routes.GetAreaName(), routes.GetOptionalString("controller"));
            if (controllerType == null)
                return true;

            return VerifyController(node, routes, controllerType);
        }

        protected virtual RouteData FindRoutesForNode(SiteMapNode node, HttpContextBase httpContext)
        {
            RouteData routeData = null;

            // create a uri for the current node.  If we have an absoluteURL,
            // it will be used instead of the baseUri
            var nodeUri = new Uri(httpContext.Request.Url, node.Url);

            // create textwriter with null stream, we don't want to output anything to it
            using (var nullWriter = new StreamWriter(Stream.Null))
            {
                // create a new http context using the node's URL instead of the current one.
                var nodeRequest = new HttpRequest(string.Empty, nodeUri.ToString(), nodeUri.Query);
                var nodeResponse = new HttpResponse(nullWriter);
                var nodeContext = new HttpContext(nodeRequest, nodeResponse);
                var nodeHttpContext = new HttpContextWrapper(nodeContext);

                var routes = RouteTable.Routes;
                //if(!string.IsNullOrEmpty(node.Route))
                //{
                //    routeData = routes[node.Route].GetRouteData(nodeHttpContext);
                //}
                //else
                //{
                routeData = routes.GetRouteData(nodeHttpContext);
                //}

                return routeData;
            }
        }

        protected virtual bool VerifyController(SiteMapNode node, RouteData routes, Type controllerType)
        {
            // Get controller factory
            var controllerFactory = ControllerBuilder.Current.GetControllerFactory();

            // Create controller context
            bool factoryBuiltController = false;
            var controllerContext = this.CreateControllerContext(node, routes, controllerType, controllerFactory, out factoryBuiltController);
            try
            {
                return this.VerifyControllerAttributes(routes, controllerType, controllerContext);
            }
            finally
            {
                // Release controller
                if (factoryBuiltController)
                    controllerFactory.ReleaseController(controllerContext.Controller);
            }
        }

        protected virtual ControllerContext CreateControllerContext(SiteMapNode node, RouteData routes, Type controllerType, IControllerFactory controllerFactory, out bool factoryBuiltController)
        {
            var httpContextBase = new HttpContextWrapper(HttpContext.Current);
            var requestContext = new RequestContext(httpContextBase, routes);
            ControllerBase controller = null;
            string controllerName = requestContext.RouteData.GetOptionalString("controller");

            // Whether controller is built by the ControllerFactory (or otherwise by Activator)
            factoryBuiltController = TryCreateController(requestContext, controllerName, controllerFactory, out controller);
            if (!factoryBuiltController)
            {
                TryCreateController(controllerType, out controller);
            }

            // Create controller context
            var controllerContext = new ControllerContext(requestContext, controller);

            // set controller's ControllerContext property for MVC
            controllerContext.Controller.ControllerContext = controllerContext;

            return controllerContext;
        }

        protected virtual bool TryCreateController(RequestContext requestContext, string controllerName, IControllerFactory controllerFactory, out ControllerBase controller)
        {
            controller = null;
            if (controllerFactory != null)
            {
                try
                {
                    controller = controllerFactory.CreateController(requestContext, controllerName) as ControllerBase;
                    if (controller != null)
                        return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        protected virtual bool TryCreateController(Type controllerType, out ControllerBase controller)
        {
            controller = null;
            try
            {
                controller = Activator.CreateInstance(controllerType) as ControllerBase;
                if (controller != null)
                    return true;
            }
            catch
            {
                return false;
            }
            return false;
        }

        protected virtual bool VerifyControllerAttributes(RouteData routes, Type controllerType, ControllerContext controllerContext)
        {
            // Get controller descriptor
            var controllerDescriptor = ControllerDescriptorFactory.Create(controllerType);
            if (controllerDescriptor == null)
                return true;

            // Get action descriptor
            var actionDescriptor = this.GetActionDescriptor(routes.GetOptionalString("action"), controllerDescriptor, controllerContext);
            if (actionDescriptor == null)
                return true;

            // Verify security
            var authorizeAttributes = this.GetAuthorizeAttributes(actionDescriptor, controllerContext);
            return this.VerifyAuthorizeAttributes(authorizeAttributes, controllerContext, actionDescriptor);
        }

        protected virtual bool VerifyAuthorizeAttributes(IEnumerable<AuthorizeAttribute> authorizeAttributes, ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            // Verify all attributes
            foreach (var authorizeAttribute in authorizeAttributes)
            {
                try
                {
                    var authorized = this.VerifyAuthorizeAttribute(authorizeAttribute, controllerContext, actionDescriptor);
                    if (!authorized)
                        return false;
                }
                catch
                {
                    // do not allow on exception
                    return false;
                }
            }
            return true;
        }

        protected virtual IEnumerable<AuthorizeAttribute> GetAuthorizeAttributes(ActionDescriptor actionDescriptor, ControllerContext controllerContext)
        {
            var filters = FilterProviders.Providers.GetFilters(controllerContext, actionDescriptor);

            return filters
                    .Where(f => typeof(AuthorizeAttribute).IsAssignableFrom(f.Instance.GetType()))
                    .Select(f => f.Instance as AuthorizeAttribute);
        }

        protected virtual bool VerifyAuthorizeAttribute(AuthorizeAttribute authorizeAttribute, ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var authorizationContext = new AuthorizationContext(controllerContext, actionDescriptor);
            authorizeAttribute.OnAuthorization(authorizationContext);
            if (authorizationContext.Result != null)
                return false;
            return true;
        }

        protected virtual ActionDescriptor GetActionDescriptor(string actionName, ControllerDescriptor controllerDescriptor, ControllerContext controllerContext)
        {
            ActionDescriptor actionDescriptor = null;
            var found = this.TryFindActionDescriptor(actionName, controllerContext, controllerDescriptor, out actionDescriptor);
            if (!found)
            {
                actionDescriptor = controllerDescriptor.GetCanonicalActions().FirstOrDefault(a => a.ActionName == actionName);
            }
            return actionDescriptor;
        }

        protected virtual bool TryFindActionDescriptor(string actionName, ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, out ActionDescriptor actionDescriptor)
        {
            actionDescriptor = null;
            try
            {
                var actionSelector = new ActionSelector();
                actionDescriptor = actionSelector.FindAction(controllerContext, controllerDescriptor, actionName);
                if (actionDescriptor != null)
                    return true;
            }
            catch
            {
                return false;
            }
            return false;
        }

        #endregion

        private class ActionSelector
            : AsyncControllerActionInvoker
        {
            // Needed because FindAction is protected, and we are changing it to be public
            public new ActionDescriptor FindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName)
            {
                return base.FindAction(controllerContext, controllerDescriptor, actionName);
            }
        }
    }
}
