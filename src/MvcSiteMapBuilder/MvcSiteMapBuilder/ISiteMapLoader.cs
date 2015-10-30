namespace MvcSiteMapBuilder
{
    public interface ISiteMapLoader
    {
        SiteMap GetSiteMap(string siteMapCacheKey = null);
        void ReleaseSiteMap();
        void ReleaseSiteMap(string siteMapCacheKey);
    }
}
