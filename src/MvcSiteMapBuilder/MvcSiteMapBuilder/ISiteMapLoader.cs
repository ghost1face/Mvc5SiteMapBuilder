﻿namespace MvcSiteMapBuilder
{
    public interface ISiteMapLoader
    {
        SiteMap GetSiteMap(string siteMapCacheKey = null);
        void ReleaseSiteMap(string siteMapCacheKey = null);
    }
}
