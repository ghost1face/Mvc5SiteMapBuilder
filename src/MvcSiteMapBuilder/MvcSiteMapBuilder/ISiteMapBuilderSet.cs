using MvcSiteMapBuilder.Cache;
using MvcSiteMapBuilder.DataSource;

namespace MvcSiteMapBuilder
{
    public interface ISiteMapBuilderSet
    {
        ICacheDetails CacheDetails { get; }
        ISiteMapDataSource DataSource { get; }   
        string BuilderSetName { get; }   
        bool AppliesTo(string builderSetName);
    }
}
