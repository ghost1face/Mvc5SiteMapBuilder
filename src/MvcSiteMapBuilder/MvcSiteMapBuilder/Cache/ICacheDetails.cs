using System;

namespace Mvc5SiteMapBuilder.Cache
{
    public interface ICacheDetails
    {
        TimeSpan AbsoluteCacheExpiration { get; }
        TimeSpan SlidingCacheExpiration { get; }
        ICacheDependency CacheDependency { get; }
    }
}
