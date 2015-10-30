namespace MvcSiteMapBuilder
{
    public interface ISiteMapBuilder
    {
        SiteMap BuildSiteMap(ISiteMapBuilderSet builderSet, string cacheKey);
    }
}
