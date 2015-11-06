using System.Xml.Linq;

namespace Mvc5SiteMapBuilder.DataSource
{
    interface ISiteMapXmlDataSource : ISiteMapDataSource
    {
        new XDocument GetSiteMapData();
    }
}
