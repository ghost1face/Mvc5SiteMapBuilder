using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;
using MvcSiteMapBuilder.Web;

namespace MvcSiteMapBuilder.Extensions
{
    public static class SiteMapExtensions
    {
        public static SiteMapNode FindSiteMapNodeFromCurrentContext(this SiteMap siteMap)
        {
            var httpContext = HttpContext.Current;
            var httpContextBase = new HttpContextWrapper(httpContext);

            return FindSiteMapNode(siteMap, httpContextBase);
        }

        public static SiteMapNode FindSiteMapNode(this SiteMap siteMap, HttpContextBase httpContext)
        {
            var node = FindSiteMapNodeFromMvc(siteMap, httpContext);

            // Check accessibility
            if (node == null || !node.IsAccessibleToUser())
            {
                return null;
            }
            return node;
        }

        //public static SiteMapNode FindSiteMapNodeFromPublicFacingUrl(this SiteMap siteMap, HttpContextBase httpContext)
        //{
        //    var publicFacingUrl = UrlPath.GetPublicFacingUrl(httpContext);
        //    return FindSiteMapNodeFromUrl(siteMap, publicFacingUrl.PathAndQuery, publicFacingUrl.AbsolutePath, publicFacingUrl.Host, httpContext.CurrentHandler);
        //}

        //public static SiteMapNode FindSiteMapNodeFromUrl(this SiteMap siteMap, string relativeUrl, string relativePath, string hostName, IHttpHandler handler)
        //{
        //    SiteMapNode node = null;

        //    // Try absolute match with querystring
        //    var absoluteMatch = siteMapChildStateFactory.CreateUrlKey(relativeUrl, hostName);
        //    node = FindSiteMapNodeFromUrlMatch(absoluteMatch);

        //    // Try absolute match without querystring
        //    if (node == null && !string.IsNullOrEmpty(relativePath))
        //    {
        //        var absoluteMatchWithoutQueryString = siteMapChildStateFactory.CreateUrlKey(relativePath, hostName);
        //        node = FindSiteMapNodeFromUrlMatch(absoluteMatchWithoutQueryString);
        //    }

        //    // Try relative match
        //    if (node == null)
        //    {
        //        var relativeMatch = siteMapChildStateFactory.CreateUrlKey(relativeUrl, string.Empty);
        //        node = FindSiteMapNodeFromUrlMatch(relativeMatch);
        //    }

        //    // Try relative match with ASP.NET handler querystring
        //    if (node == null)
        //    {
        //        Page currentHandler = handler as Page;
        //        if (currentHandler != null)
        //        {
        //            string clientQueryString = currentHandler.ClientQueryString;
        //            if (clientQueryString.Length > 0)
        //            {
        //                var aspNetRelativeMatch = siteMapChildStateFactory.CreateUrlKey(relativePath + "?" + clientQueryString, string.Empty);
        //                node = FindSiteMapNodeFromUrlMatch(aspNetRelativeMatch);
        //            }
        //        }
        //    }

        //    // Try relative match without querystring
        //    if (node == null && !string.IsNullOrEmpty(relativePath))
        //    {
        //        var relativeMatchWithoutQueryString = siteMapChildStateFactory.CreateUrlKey(relativePath, string.Empty);
        //        node = FindSiteMapNodeFromUrlMatch(relativeMatchWithoutQueryString);
        //    }

        //    return node;
        //}

        public static SiteMapNode FindSiteMapNode(this SiteMap siteMap, string rawUrl)
        {
            if (rawUrl == null)
            {
                throw new ArgumentNullException(nameof(rawUrl));
            }
            rawUrl = rawUrl.Trim();
            if (rawUrl.Length == 0)
            {
                return null;
            }

            // NOTE: If the URL passed is absolute, the public facing URL will be ignored
            // and the current URL will be the absolute URL that is passed.
            var publicFacingUrl = UrlPath.GetPublicFacingUrl(UrlPath.HttpContext);
            var currentUrl = new Uri(publicFacingUrl, rawUrl);

            //// Search the internal dictionary for the URL that is registered manually.
            //var node = FindSiteMapNodeFromUrl(currentUrl.PathAndQuery, currentUrl.AbsolutePath, currentUrl.Host, System.Web.HttpContext.Current);
            SiteMapNode node = null;

            // Search for the URL by creating a context based on the new URL and matching route values.
            if (node == null)
            {
                // Create a TextWriter with null stream as a backing stream 
                // which doesn't consume resources
                using (var nullWriter = new StreamWriter(Stream.Null))
                {
                    // create a new http context using the node's URL instead of the current one.
                    var currentUrlRequest = new HttpRequest(string.Empty, currentUrl.ToString(), currentUrl.Query);
                    var currentUrlResponse = new HttpResponse(nullWriter);
                    var currentUrlContext = new HttpContext(currentUrlRequest, currentUrlResponse);
                    var currentUrlHttpContext = new HttpContextWrapper(currentUrlContext);

                    // Find node for the passed-in URL using the new HTTP context. This will do a
                    // match based on route values and/or query string values.
                    node = FindSiteMapNodeFromMvc(siteMap, currentUrlHttpContext);
                }
            }

            // Check accessibility
            if (node == null || !node.IsAccessibleToUser())
            {
                return null;
            }
            return node;
        }

        public static SiteMapNode FindSiteMapNodeFromMvc(this SiteMap siteMap, HttpContextBase httpContext)
        {
            var routeData = GetMvcRouteData(httpContext);
            if (routeData != null)
            {
                return FindSiteMapNodeFromMvcRoute(siteMap, routeData.Values, routeData.Route);
            }
            return null;
        }

        public static SiteMapNode FindSiteMapNodeFromMvcRoute(SiteMap siteMap, IDictionary<string, object> values, RouteBase route)
        {
            var routes = RouteTable.Routes;

            // keyTable contains every node in the SiteMap
            foreach (var node in siteMap.GetKeyToNodeDictionary().Values)
            {
                if (node.MatchesRoute(values))
                {
                    return node;
                }
            }

            return null;
        }

        private static RouteData GetMvcRouteData(HttpContextBase httpContext)
        {
            const string routeMatchKey = "MS_DirectRouteMatches";
            var routes = RouteTable.Routes;
            var routeData = routes.GetRouteData(httpContext);
            if (routeData != null)
            {
                if (routeData.Values.ContainsKey(routeMatchKey))
                {
                    routeData = ((IEnumerable<RouteData>)routeData.Values[routeMatchKey]).First();
                }
                SetMvcArea(routeData);
            }
            return routeData;
        }

        private static void SetMvcArea(RouteData routeData)
        {
            if (routeData != null)
            {
                if (!routeData.Values.ContainsKey("area"))
                {
                    routeData.Values.Add("area", routeData.GetAreaName());
                }
            }
        }
    }
}
