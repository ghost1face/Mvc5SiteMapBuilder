using Newtonsoft.Json.Linq;

namespace MvcSiteMapBuilder.DataSource
{
    public interface ISiteMapJSONDataSource : ISiteMapDataSource
    {
        new JObject GetSiteMapData();
    }
}
