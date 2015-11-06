using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc5SiteMapBuilder;
using Mvc5SiteMapBuilder.Providers;

namespace SimpleUseTestApplication.SiteMap
{
    public class CustomNodeProvider : IDynamicNodeProvider
    {
        public IEnumerable<Mvc5SiteMapBuilder.SiteMapNode> GetSiteMapNodes()
        {
            yield return new Mvc5SiteMapBuilder.SiteMapNode
            {
                Action = "Index",
                Controller = "Dynamic",
                Title = "Index"
            };
            yield return new Mvc5SiteMapBuilder.SiteMapNode
            {
                Action = "Page2",
                Controller = "Dynamic",
                Title = "Page2"
            };
            yield return new Mvc5SiteMapBuilder.SiteMapNode
            {
                Action = "Page3",
                Controller = "Dynamic",
                Title = "Page3"
            };
        }
    }
}