using System.Xml.Linq;

namespace MvcSiteMapBuilder.DataSource
{
    interface ISiteMapXmlDataSource : ISiteMapDataSource
    {
        new XDocument GetSiteMapData();
    }
}
