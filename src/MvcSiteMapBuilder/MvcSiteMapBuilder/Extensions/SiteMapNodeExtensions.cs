using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcSiteMapBuilder.Helpers;
using MvcSiteMapBuilder.Security;

namespace MvcSiteMapBuilder.Extensions
{
    public static class SiteMapNodeExtensions
    {
        /// <summary>
        /// Determine if node is accessible to end user
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsAccessibleToUser(this SiteMapNode node)
        {
            var aclModule = SiteMapConfiguration.Instance.Container.Resolve<IAclModule>();

            return aclModule.IsAccessibleToUser(node);
        }

        /// <summary>
        /// Determines if node url is within the application or external
        /// </summary>
        /// <param name="node"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static bool HasExternalUrl(this SiteMapNode node, HttpContextBase httpContext)
        {
            string url;
            if (!string.IsNullOrEmpty(node.Url))
                url = node.Url;
            else
            {
                url = node.Url = node.ResolveUrl(httpContext);
            }

            if (!UrlHelpers.IsAbsoluteUrl(url))
                return false;

            Uri uri = null;
            if(Uri.TryCreate(url, UriKind.Absolute, out uri))
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
        /// <param name="node"></param>
        /// <returns></returns>
        public static string ResolveUrl(this SiteMapNode node)
        {
            var httpContext = HttpContext.Current;
            var httpContextBase = new HttpContextWrapper(httpContext);

            return node.ResolveUrl(httpContextBase);
        }

        /// <summary>
        /// Resolves url field through route parameters
        /// </summary>
        /// <param name="node"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string ResolveUrl(this SiteMapNode node, HttpContextBase httpContext)
        {
            var urlHelper = new UrlHelper(new RequestContext(httpContext, new RouteData()));

            return urlHelper.Action(node.Action, node.Controller, new { area = node.Area });
        }

        /// <summary>
        /// Determines if the node is visible
        /// </summary>
        /// <param name="node"></param>
        /// <param name="sourceMetaData"></param>
        /// <returns></returns>
        public static bool IsVisible(this SiteMapNode node, IDictionary<string, object> sourceMetaData)
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
            foreach(var node in siteMap.Nodes)
            {
                return FindParentNode(node, siteMapNode);
            }
            return null;
        }

        #region Private Methods

        private static SiteMapNode FindParentNode(SiteMapNode parentNode, SiteMapNode siteMapNodetoFind)
        {
            if (!parentNode.HasChildNodes)
                return null;

            foreach(var childNode in parentNode.ChildNodes)
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
