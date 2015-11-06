using Newtonsoft.Json.Linq;

namespace Mvc5SiteMapBuilder.DataSource
{
    public interface ISiteMapJSONDataSource : ISiteMapDataSource
    {
        new JObject GetSiteMapData();
    }
}
