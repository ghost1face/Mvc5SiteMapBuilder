using System.IO;
using System.Xml;

namespace MvcSiteMapBuilder.DataSource
{
    public interface ISiteMapDataSource
    {
        object GetSiteMapData();
    }
}
