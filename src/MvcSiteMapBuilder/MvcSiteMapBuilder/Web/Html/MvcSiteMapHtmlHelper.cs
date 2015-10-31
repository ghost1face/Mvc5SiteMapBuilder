﻿using System;
using System.Web.Mvc;
using MvcSiteMapBuilder.Web.Mvc;

namespace MvcSiteMapBuilder.Web.Html
{
    /// <summary>
    /// MvcSiteMapHtmlHelper class
    /// </summary>
    public class MvcSiteMapHtmlHelper
    {
        /// <summary>
        /// Gets or sets the HTML helper.
        /// </summary>
        /// <value>The HTML helper.</value>
        public HtmlHelper HtmlHelper { get; protected set; }

        /// <summary>
        /// Gets or sets the sitemap.
        /// </summary>
        /// <value>The sitemap.</value>
        public SiteMap SiteMap { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcSiteMapHtmlHelper"/> class.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="siteMap">The sitemap.</param>
        public MvcSiteMapHtmlHelper(HtmlHelper htmlHelper, SiteMap siteMap)
            : this(htmlHelper, siteMap, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcSiteMapHtmlHelper"/> class.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="siteMap">The sitemap.</param>
        /// <param name="useViewEngine"><c>true</c> to use the internal view engine; otherwise <c>false</c></param>
        internal MvcSiteMapHtmlHelper(HtmlHelper htmlHelper, SiteMap siteMap, bool useViewEngine)
        {
            if (htmlHelper == null)
                throw new ArgumentNullException(nameof(htmlHelper));
            if (siteMap == null)
                throw new ArgumentNullException(nameof(siteMap));

            HtmlHelper = htmlHelper;
            SiteMap = siteMap;

            if (useViewEngine)
                MvcSiteMapProviderViewEngine.Register();
        }

        /// <summary>
        /// Creates the HTML helper for model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public HtmlHelper<TModel> CreateHtmlHelperForModel<TModel>(TModel model)
        {
            return new HtmlHelper<TModel>(HtmlHelper.ViewContext, new ViewDataContainer<TModel>(model));
        }
    }
}
