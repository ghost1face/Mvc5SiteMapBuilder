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

        public SiteMapNode CurrentNode
        {
            get
            {
                var currentNode = this.FindSiteMapNodeFromCurrentContext();
                if (currentNode != null && currentNode.IsAccessibleToUser())
                    return currentNode;
                return RootNode;
            }
        }

        public IDictionary<string, SiteMapNode> GetKeyToNodeDictionary()
        {
            // hack for testing, this assumes max hierarchy level is 4
            var enumerableNodes =
                Nodes.Select(i => i)
                    .Concat(Nodes.SelectMany(i => i.ChildNodes))
                    .Concat(Nodes.SelectMany(i => i.ChildNodes.SelectMany(j => j.ChildNodes)))
                    .Concat(Nodes.SelectMany(i => i.ChildNodes.SelectMany(j => j.ChildNodes.SelectMany(k => k.ChildNodes))));

            return enumerableNodes.ToDictionary(k => k.Key, v => v);
        }
    }
}
