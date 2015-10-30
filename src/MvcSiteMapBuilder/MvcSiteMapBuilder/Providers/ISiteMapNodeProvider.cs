using System.Collections.Generic;
using MvcSiteMapBuilder.DataSource;

namespace MvcSiteMapBuilder.Providers
{
    public interface ISiteMapNodeProvider
    {
        IEnumerable<SiteMapNode> GetSiteMapNodes(ISiteMapDataSource dataSource);
    }
}
