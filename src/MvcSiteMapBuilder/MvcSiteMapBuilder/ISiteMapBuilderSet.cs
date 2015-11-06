using Mvc5SiteMapBuilder.Cache;
using Mvc5SiteMapBuilder.DataSource;

namespace Mvc5SiteMapBuilder
{
    public interface ISiteMapBuilderSet
    {
        ICacheDetails CacheDetails { get; }
        ISiteMapDataSource DataSource { get; }   
        string BuilderSetName { get; }   
        bool AppliesTo(string builderSetName);
    }
}
