using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcSiteMapBuilder;
using MvcSiteMapBuilder.Providers;

namespace SimpleUseTestApplication.SiteMap
{
    public class CustomNodeProvider : IDynamicNodeProvider
    {
        public IEnumerable<MvcSiteMapBuilder.SiteMapNode> GetSiteMapNodes()
        {
            yield return new MvcSiteMapBuilder.SiteMapNode
            {
                Action = "Index",
                Controller = "Dynamic",
                Title = "Index"
            };
            yield return new MvcSiteMapBuilder.SiteMapNode
            {
                Action = "Page2",
                Controller = "Dynamic",
                Title = "Page2"
            };
            yield return new MvcSiteMapBuilder.SiteMapNode
            {
                Action = "Page3",
                Controller = "Dynamic",
                Title = "Page3"
            };
        }
    }
}