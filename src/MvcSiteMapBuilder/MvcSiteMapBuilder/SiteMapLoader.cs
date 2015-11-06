﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mvc5SiteMapBuilder.Cache;
using Mvc5SiteMapBuilder.Extensions;

namespace Mvc5SiteMapBuilder
{
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

            // retrieve builderset
            var builderSet = GetBuilderSet(siteMapCacheKey);

            // check cache for sitemap if not available generate using builder
            // cache using cachedetails
            var siteMap = siteMapCache.GetOrAdd(
                siteMapCacheKey,
                () => siteMapBuilder.BuildSiteMap(builderSet, siteMapCacheKey),
                builderSet.CacheDetails
            );

            // create new restricted sitemap based on user's permissions
            var restrictedSiteMap = VisitSiteMap(siteMap);

            // return new filtered sitemap
            return restrictedSiteMap;
        }

        public void ReleaseSiteMap(string siteMapCacheKey = null)
        {
            if (string.IsNullOrEmpty(siteMapCacheKey))
                siteMapCacheKey = siteMapCacheKeyGenerator.GenerateKey();

            siteMapCache.Remove(siteMapCacheKey);
        }

        #region Private Methods

        protected virtual ISiteMapBuilderSet GetBuilderSet(string siteMapCacheKey)
        {
            var builderSetName = siteMapCacheKeyToBuilderSetMapper.GetBuilderSetName(siteMapCacheKey);
            var builderSet = siteMapBuilderSetStrategy.GetBuilderSet(builderSetName);

            return builderSet;
        }

        protected virtual SiteMap VisitSiteMap(SiteMap siteMap)
        {
            var siteMapNodes = new List<SiteMapNode>();

            if (siteMap.Nodes != null)
            {
                // copy all root nodes and children, this makes all subsequent siteMap modifications only for this cloned instance
                // this solves an issue of an inmemory cacheable object
                foreach (var node in siteMap.Nodes.Select(n => n.Copy()))
                {
                    if (IsNodeAccessible(node))
                        siteMapNodes.Add(node);
                }
            }

            return new SiteMap
            {
                CacheKey = siteMap.CacheKey,
                Nodes = siteMapNodes
            };
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
