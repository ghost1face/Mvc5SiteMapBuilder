using System.Collections.Generic;
using Mvc5SiteMapBuilder.DataSource;

namespace Mvc5SiteMapBuilder.Providers
{
    public interface ISiteMapNodeProvider
    {
        IEnumerable<SiteMapNode> GetSiteMapNodes(ISiteMapDataSource dataSource);
    }
}
