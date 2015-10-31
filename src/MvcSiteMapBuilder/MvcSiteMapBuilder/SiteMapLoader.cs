using System;
using System.Collections.Generic;
using System.Linq;
using MvcSiteMapBuilder.Cache;
using MvcSiteMapBuilder.Extensions;

namespace MvcSiteMapBuilder
{
    // TODO: 
    //      Cache & retrieve sitemap

    public class SiteMapLoader : ISiteMapLoader
    {
        private readonly ISiteMapBuilder siteMapBuilder;
        private readonly ISiteMapCacheKeyGenerator siteMapCacheKeyGenerator;
        private readonly ISiteMapCacheKeyToBuilderSetMapper siteMapCacheKeyToBuilderSetMapper;
        private readonly ISiteMapBuilderSetStrategy siteMapBuilderSetStrategy;
        private readonly ISiteMapCache siteMapCache;

        public SiteMapLoader(ISiteMapBuilder siteMapBuilder, ISiteMapCacheKeyGenerator siteMapCacheKeyGenerator,
            ISiteMapCacheKeyToBuilderSetMapper siteMapCacheKeyToBuilderSetMapper, ISiteMapBuilderSetStrategy siteMapBuilderSetStrategy, ISiteMapCache siteMapCache)
        {
            if (siteMapBuilder == null)
                throw new ArgumentNullException(nameof(siteMapBuilder));

            if (siteMapCacheKeyGenerator == null)
                throw new ArgumentNullException(nameof(siteMapCacheKeyGenerator));

            if (siteMapCacheKeyToBuilderSetMapper == null)
                throw new ArgumentNullException(nameof(siteMapCacheKeyToBuilderSetMapper));

            if (siteMapBuilderSetStrategy == null)
                throw new ArgumentNullException(nameof(siteMapBuilderSetStrategy));

            if (siteMapCache == null)
                throw new ArgumentNullException(nameof(siteMapCache));

            this.siteMapBuilder = siteMapBuilder;
            this.siteMapCacheKeyGenerator = siteMapCacheKeyGenerator;
            this.siteMapCacheKeyToBuilderSetMapper = siteMapCacheKeyToBuilderSetMapper;
            this.siteMapBuilderSetStrategy = siteMapBuilderSetStrategy;
            this.siteMapCache = siteMapCache;
        }

        public SiteMap GetSiteMap(string siteMapCacheKey = null)
        {
            if (string.IsNullOrEmpty(siteMapCacheKey))
                siteMapCacheKey = siteMapCacheKeyGenerator.GenerateKey();

            var builderSet = GetBuilderSet(siteMapCacheKey);

            var siteMap = siteMapCache.GetOrAdd(
                siteMapCacheKey,
                () => siteMapBuilder.BuildSiteMap(builderSet, siteMapCacheKey),
                builderSet.CacheDetails
            );

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

            if (rootNode.ChildNodes.Any())
            {
                var itemCount = rootNode.ChildNodes.Count - 1;
                for (var i = itemCount; i >= 0; i--)
                {
                    var currentNode = rootNode.ChildNodes[i];

                    if (!IsNodeAccessible(currentNode))
                        rootNode.ChildNodes.RemoveAt(i);
                }
            }

            return true;
        }

        #endregion
    }
}
