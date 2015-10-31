using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MvcSiteMapBuilder.Extensions;

namespace MvcSiteMapBuilder
{
    public class SiteMap
    {
        public bool VisibilityAffectsDescendants { get; } = true;
        public string CacheKey { get; set; }
        public List<SiteMapNode> Nodes { get; set; }

        public SiteMapNode RootNode
        {
            get
            {
                if (Nodes == null || !Nodes.Any())
                    return null;

                var rootNode = Nodes.FirstOrDefault();
                if (rootNode == null)
                    return null;

                return rootNode.IsAccessibleToUser() ? rootNode : null;
            }
        }
    }
}
