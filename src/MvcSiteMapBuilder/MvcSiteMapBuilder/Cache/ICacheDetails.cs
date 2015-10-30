using System;

namespace MvcSiteMapBuilder.Cache
{
    public interface ICacheDetails
    {
        TimeSpan AbsoluteCacheExpiration { get; }
        TimeSpan SlidingCacheExpiration { get; }
        ICacheDependency CacheDependency { get; }
    }
}
