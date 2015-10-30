using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Copies an existing sitemap node and all properties
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static SiteMapNode Copy(this SiteMapNode node)
        {
            return new SiteMapNode
            {
                Action = node.Action,
                Area  = node.Area,
                Attributes = new Dictionary<string, string>(node.Attributes),
                Children = node.Children.Select(n => n.Copy()).ToList(),
                Clickable = node.Clickable,
                Controller = node.Controller,
                DynamicNodeProvider = node.DynamicNodeProvider,
                Key = node.Key,
                Title = node.Title,
                Url = node.Url
            };
        }
    }
}
