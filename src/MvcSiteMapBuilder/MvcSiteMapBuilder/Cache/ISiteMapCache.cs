namespace MvcSiteMapBuilder.Cache
{
    public interface ISiteMapCache
    {
        bool Add<T>(string siteMapCacheKey, T item, ICacheDetails cacheDetails);
        T Get<T>(string siteMapCacheKey);
    }
}
