using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MvcSiteMapBuilder.Helpers
{
    public static class UrlHelpers
    {
        /// <summary>
        /// Determines if the URL is an absolute or relative URL.
        /// </summary>
        /// <param name="url">Any Url including those starting with "/", "~", or protocol.</param>
        /// <returns><b>true</b> if the URL is absolute; otherwise <b>false</b>.</returns>
        public static bool IsAbsoluteUrl(string url)
        {
            // Optimization: Return false early if there is no scheme delimiter in the string
            // prefixed by at least 1 character.
            if (!(url.IndexOf(Uri.SchemeDelimiter, StringComparison.Ordinal) > 0))
                return false;

            // There must be at least 1 word character before the scheme delimiter.
            // This ensures we don't return true for query strings that contain absolute URLs.
            return Regex.IsMatch(url, @"^\w+://", RegexOptions.Compiled);
        }

        /// <summary>
        /// Gets the public facing URL for the given incoming HTTP request.
        /// </summary>
        /// <param name="httpContext">The HTTP context representing the context of the request.</param>
        /// <returns>The URI that the outside world used to create this request.</returns>
        /// <remarks>Source: http://stackoverflow.com/questions/7795910/how-do-i-get-url-action-to-use-the-right-port-number#11888846 </remarks>
        public static Uri GetPublicFacingUrl(HttpContextBase httpContext)
        {
            var serverVariables = httpContext.Request.ServerVariables;
            var request = httpContext.Request;

            // Due to URL rewriting, cloud computing (i.e. Azure)
            // and web farms, etc., we have to be VERY careful about what
            // we consider the incoming URL.  We want to see the URL as it would
            // appear on the public-facing side of the hosting web site.
            // HttpRequest.Url gives us the internal URL in a cloud environment,
            // So we use a variable that (at least from what I can tell) gives us
            // the public URL:
            if (serverVariables["HTTP_HOST"] != null)
            {
                string scheme = serverVariables["HTTP_X_FORWARDED_PROTO"] ?? request.Url.Scheme;
                var hostAndPort = new Uri(scheme + Uri.SchemeDelimiter + serverVariables["HTTP_HOST"]);
                var publicRequestUri = new UriBuilder(request.Url);
                publicRequestUri.Scheme = scheme;
                publicRequestUri.Host = hostAndPort.Host;
                publicRequestUri.Port = hostAndPort.Port; // CC missing Uri.Port contract that's on UriBuilder.Port
                return publicRequestUri.Uri;
            }
            // Failover to the method that works for non-web farm environments.
            // We use Request.Url for the full path to the server, and modify it
            // with Request.RawUrl to capture both the cookieless session "directory" if it exists
            // and the original path in case URL rewriting is going on.  We don't want to be
            // fooled by URL rewriting because we're comparing the actual URL with what's in
            // the return_to parameter in some cases.
            // Response.ApplyAppPathModifier(builder.Path) would have worked for the cookieless
            // session, but not the URL rewriting problem.
            return new Uri(request.Url, request.RawUrl);
        }
    }
}
