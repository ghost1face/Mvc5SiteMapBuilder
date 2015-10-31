using System;

namespace MvcSiteMapBuilder
{
    /// <summary>
    /// This class is the static entry point where the presentation layer can request a sitemap by calling Current or passing a siteMapCacheKey.
    /// </summary>
    public static class SiteMaps
    {
        private static ISiteMapLoader loader;

        public static ISiteMapLoader Loader
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (loader != null)
                    throw new Exception("ISiteMapLoader instance has already been set.");
                loader = value;
            }
        }

        public static SiteMap Current
        {
            get { return GetSiteMap(); }
        }

        public static SiteMap GetSiteMap(string siteMapCacheKey)
        {
            ThrowIfLoaderNotInitialized();
            return loader.GetSiteMap(siteMapCacheKey);
        }

        public static SiteMap GetSiteMap()
        {
            ThrowIfLoaderNotInitialized();
            return loader.GetSiteMap();
        }

        public static void ReleaseSiteMap(string siteMapCacheKey)
        {
            ThrowIfLoaderNotInitialized();
            loader.ReleaseSiteMap(siteMapCacheKey);
        }

        public static void ReleaseSiteMap()
        {
            ThrowIfLoaderNotInitialized();
            loader.ReleaseSiteMap();
        }

        private static void ThrowIfLoaderNotInitialized()
        {
            if (loader == null)
            {
                throw new Exception("ISiteMapLoader not initialized.");
            }
        }
    }
}
