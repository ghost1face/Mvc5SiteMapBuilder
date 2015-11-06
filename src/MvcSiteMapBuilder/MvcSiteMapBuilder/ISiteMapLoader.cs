namespace Mvc5SiteMapBuilder
{
    public interface ISiteMapLoader
    {
        SiteMap GetSiteMap(string siteMapCacheKey = null);
        void ReleaseSiteMap(string siteMapCacheKey = null);
    }
}
