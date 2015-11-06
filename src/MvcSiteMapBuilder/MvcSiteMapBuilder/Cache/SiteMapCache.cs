﻿using System;

namespace Mvc5SiteMapBuilder.Cache
{
    public class SiteMapCache : ISiteMapCache
    {
        private readonly ICacheProvider<SiteMap> cacheProvider;

        public SiteMapCache(ICacheProvider<SiteMap> cacheProvider)
        {
            this.cacheProvider = cacheProvider;
        }

        public SiteMap GetOrAdd(string siteMapCacheKey, Func<SiteMap> createFunction, ICacheDetails cacheDetails)
        {
            SiteMap siteMap;
            var success = cacheProvider.TryGetValue(siteMapCacheKey, out siteMap);

            if (!success)
            {
                System.Diagnostics.Debug.WriteLine($"Cache Miss: {siteMapCacheKey}");
                siteMap = createFunction();
                cacheProvider.Add(siteMapCacheKey, siteMap, cacheDetails);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Cache Hit: {siteMapCacheKey}");
            }

            return siteMap;
        }

        public void Remove(string siteMapCacheKey)
        {
            cacheProvider.Remove(siteMapCacheKey);
        }

        public bool Contains(string key)
        {
            return cacheProvider.Contains(key);
        }
    }
}
