using System;
using MvcSiteMapBuilder.Cache;
using MvcSiteMapBuilder.Security;
using MvcSiteMapBuilder.Extensions;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace MvcSiteMapBuilder
{
    // TODO: Build SiteMap
    //      Cache & retrieve sitemap

    public class SiteMapLoader : ISiteMapLoader
    {
        private readonly ISiteMapBuilder siteMapBuilder;
        private readonly ISiteMapCacheKeyGenerator siteMapCacheKeyGenerator;
        private readonly ISiteMapCacheKeyToBuilderSetMapper siteMapCacheKeyToBuilderSetMapper;
        private readonly ISiteMapBuilderSetStrategy siteMapBuilderSetStrategy;

        public SiteMapLoader(ISiteMapBuilder siteMapBuilder, ISiteMapCacheKeyGenerator siteMapCacheKeyGenerator,
            ISiteMapCacheKeyToBuilderSetMapper siteMapCacheKeyToBuilderSetMapper, ISiteMapBuilderSetStrategy siteMapBuilderSetStrategy)
        {
            this.siteMapBuilder = siteMapBuilder;
            this.siteMapCacheKeyGenerator = siteMapCacheKeyGenerator;
            this.siteMapCacheKeyToBuilderSetMapper = siteMapCacheKeyToBuilderSetMapper;
            this.siteMapBuilderSetStrategy = siteMapBuilderSetStrategy;
        }

        public SiteMap GetSiteMap(string siteMapCacheKey = null)
        {
            var builderSet = GetBuilderSet(siteMapCacheKey);

            var siteMap = siteMapBuilder.BuildSiteMap(builderSet, siteMapCacheKey);

            VisitSiteMap(siteMap);

            return siteMap;
        }

        public void ReleaseSiteMap()
        {
            throw new NotImplementedException();
        }

        public void ReleaseSiteMap(string siteMapCacheKey)
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        protected virtual ISiteMapBuilderSet GetBuilderSet(string siteMapCacheKey)
        {
            var builderSetName = siteMapCacheKeyToBuilderSetMapper.GetBuilderSetName(siteMapCacheKey);
            var builderSet = siteMapBuilderSetStrategy.GetBuilderSet(builderSetName);

            return builderSet;
        }

        protected virtual void VisitSiteMap(SiteMap siteMap)
        {
            List<SiteMapNode> siteMapNodes = new List<SiteMapNode>();

            // copy all root nodes and children, this makes all subsequent siteMap modifications only for this cloned instance
            // this solves an issue of an inmemory cacheable object
            foreach (var node in siteMap.Nodes.Select(n => n.Copy())) 
            {
                if (IsNodeAccessible(node))
                    siteMapNodes.Add(node);
            }

            siteMap.Nodes = siteMapNodes;
        }

        protected virtual bool IsNodeAccessible(SiteMapNode rootNode)
        {
            if (!rootNode.IsAccessibleToUser())
                return false;

            if (rootNode.Children.Any())
            {
                var itemCount = rootNode.Children.Count - 1;
                for (var i = itemCount; i >= 0; i--)
                {
                    var currentNode = rootNode.Children[i];

                    if (!IsNodeAccessible(currentNode))
                        rootNode.Children.RemoveAt(i);
                }
            }

            return true;
        }

        #endregion
    }
}
