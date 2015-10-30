using System;

namespace MvcSiteMapBuilder.Cache
{
    public class CacheDetails : ICacheDetails
    {
        private TimeSpan absoluteCacheExpiration;
        private TimeSpan slidingCacheExpiration;
        private ICacheDependency cacheDependency;

        public CacheDetails(TimeSpan absoluteCacheExpiration, TimeSpan slidingCacheExpiration, ICacheDependency cacheDependency)
        {
            this.absoluteCacheExpiration = absoluteCacheExpiration;
            this.slidingCacheExpiration = slidingCacheExpiration;
            this.cacheDependency = cacheDependency;
        }

        public TimeSpan AbsoluteCacheExpiration
        {
            get { return absoluteCacheExpiration; }
        }

        public ICacheDependency CacheDependency
        {
            get { return cacheDependency; }
        }

        public TimeSpan SlidingCacheExpiration
        {
            get { return slidingCacheExpiration; }
        }
    }
}
