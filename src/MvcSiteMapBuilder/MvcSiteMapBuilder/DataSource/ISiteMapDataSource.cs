using System.IO;
using System.Xml;

namespace Mvc5SiteMapBuilder.DataSource
{
    public interface ISiteMapDataSource
    {
        object GetSiteMapData();
    }
}
