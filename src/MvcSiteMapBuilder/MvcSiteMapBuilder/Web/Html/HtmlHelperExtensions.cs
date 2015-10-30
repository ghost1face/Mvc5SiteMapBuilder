using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcSiteMapBuilder.Web.Html
{
    /// <summary>
    /// HtmlHelperExtensions class
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Creates a new MvcSiteMapProvider HtmlHelper.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns>
        /// A <see cref="MvcSiteMapHtmlHelper"/> instance 
        /// </returns>
        public static MvcSiteMapHtmlHelper MvcSiteMap(this HtmlHelper helper)
        {
            return new MvcSiteMapHtmlHelper(helper, SiteMaps.Current);
        }

        /// <summary>
        /// Creates a new MvcSiteMapProvider HtmlHelper.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="siteMap">The SiteMap.</param>
        /// <returns>
        /// A <see cref="MvcSiteMapHtmlHelper"/> instance
        /// </returns>
        public static MvcSiteMapHtmlHelper MvcSiteMap(this HtmlHelper helper, SiteMap siteMap)
        {
            return new MvcSiteMapHtmlHelper(helper, siteMap);
        }

        /// <summary>
        /// Creates a new MvcSiteMapProvider HtmlHelper.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="siteMapCacheKey">The SiteMap Cache Key.</param>
        /// <returns>
        /// A <see cref="MvcSiteMapHtmlHelper"/> instance
        /// </returns>
        public static MvcSiteMapHtmlHelper MvcSiteMap(this HtmlHelper helper, string siteMapCacheKey)
        {
            SiteMap siteMap = SiteMaps.GetSiteMap(siteMapCacheKey);
            if (siteMap == null)
                throw new Exception("Unknown sitemap.");
            return MvcSiteMap(helper, siteMap);
        }

    }
}
