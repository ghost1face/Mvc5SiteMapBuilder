using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Mvc5SiteMapBuilder.Helpers;
using Mvc5SiteMapBuilder.Security;

namespace Mvc5SiteMapBuilder.Extensions
{
    public static class SiteMapNodeExtensions
    {
        /// <summary>
        /// Determine if node is accessible to end user
        /// </summary>
        /// <param name="siteMapNode"></param>
        /// <returns></returns>
        public static bool IsAccessibleToUser(this SiteMapNode siteMapNode)
        {
            var aclModule = SiteMapConfiguration.Instance.Container.Resolve<IAclModule>();

            return aclModule.IsAccessibleToUser(siteMapNode);
        }

        /// <summary>
        /// Determines if node url is within the application or external
        /// </summary>
        /// <param name="siteMapNode"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static bool HasExternalUrl(this SiteMapNode siteMapNode, HttpContextBase httpContext)
        {
            string url;
            if (!string.IsNullOrEmpty(siteMapNode.Url))
                url = siteMapNode.Url;
            else
            {
                url = siteMapNode.Url = siteMapNode.ResolveUrl(httpContext);
            }

            if (!UrlHelpers.IsAbsoluteUrl(url))
                return false;

            Uri uri = null;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                var publicFacingUrl = UrlHelpers.GetPublicFacingUrl(httpContext);
                var isDifferentHost = !uri.Host.Equals(publicFacingUrl.Host, StringComparison.InvariantCultureIgnoreCase);
                var isDifferentVirtualApplication = !uri.AbsolutePath.StartsWith(httpContext.Request.ApplicationPath, StringComparison.InvariantCultureIgnoreCase);

                return isDifferentHost || isDifferentVirtualApplication;
            }

            return false;
        }

        /// <summary>
        /// Resolve url on sitemap node
        /// </summary>
        /// <param name="siteMapNode"></param>
        /// <returns></returns>
        public static string ResolveUrl(this SiteMapNode siteMapNode)
        {
            var httpContext = HttpContext.Current;
            var httpContextBase = new HttpContextWrapper(httpContext);

            return siteMapNode.ResolveUrl(httpContextBase);
        }

        /// <summary>
        /// Resolves url field through route parameters
        /// </summary>
        /// <param name="siteMapNode"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string ResolveUrl(this SiteMapNode siteMapNode, HttpContextBase httpContext)
        {
            var urlHelper = new UrlHelper(new RequestContext(httpContext, new RouteData()));

            return urlHelper.Action(siteMapNode.Action, siteMapNode.Controller, new { area = siteMapNode.Area });
        }

        /// <summary>
        /// Determines if the node is visible
        /// </summary>
        /// <param name="siteMapNode"></param>
        /// <param name="sourceMetaData"></param>
        /// <returns></returns>
        public static bool IsVisible(this SiteMapNode siteMapNode, IDictionary<string, object> sourceMetaData)
        {
            return true;
        }

        /// <summary>
        /// Finds the parent node of the specified node
        /// </summary>
        /// <param name="siteMapNode"></param>
        /// <param name="siteMap"></param>
        /// <returns></returns>
        public static SiteMapNode GetParentNode(this SiteMapNode siteMapNode, SiteMap siteMap)
        {
            foreach (var node in siteMap.Nodes)
            {
                return FindParentNode(node, siteMapNode);
            }
            return null;
        }

        public static bool MatchesRoute(this SiteMapNode siteMapNode, IDictionary<string, object> routeValues)
        {
            // if not clickable we never want to match the node
            if (!siteMapNode.Clickable)
                return false;

            // if URL is set explicitly, we should never match based on route values
            if (!string.IsNullOrEmpty(siteMapNode.UnresolvedUrl))
                return false;

            // TODO: Maybe querystring matching?

            object area;
            if (routeValues.TryGetValue("area", out area))
            {
                if (area.ToString() != siteMapNode.Area)
                    return false;
            }

            var controller = routeValues["controller"];
            if (controller != null && !controller.ToString().Equals(siteMapNode.Controller, StringComparison.OrdinalIgnoreCase))
                return false;

            var action = routeValues["action"];
            if (action != null && !action.ToString().Equals(siteMapNode.Action, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        #region Private Methods

        private static SiteMapNode FindParentNode(SiteMapNode parentNode, SiteMapNode siteMapNodetoFind)
        {
            if (!parentNode.HasChildNodes)
                return null;

            foreach (var childNode in parentNode.ChildNodes)
            {
                if (childNode.Key.Equals(siteMapNodetoFind.Key))
                    return parentNode;

                var newParentNode = FindParentNode(childNode, siteMapNodetoFind);
                if (newParentNode != null)
                    return newParentNode;
            }

            return null;
        }

        #endregion
    }
}
