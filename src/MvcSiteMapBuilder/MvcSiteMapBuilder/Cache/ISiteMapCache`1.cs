using System;

namespace MvcSiteMapBuilder.Cache
{
    public interface ISiteMapCache<T>
        where T : class
    {
        T GetOrAdd(string siteMapCacheKey, Func<SiteMap> createFunction, ICacheDetails cacheDetails);
        bool Contains(string key);
        void Remove(string siteMapCacheKey);
    }
}
