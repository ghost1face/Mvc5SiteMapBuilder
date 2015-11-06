namespace Mvc5SiteMapBuilder
{
    public interface ISiteMapBuilder
    {
        SiteMap BuildSiteMap(ISiteMapBuilderSet builderSet, string cacheKey);
    }
}
