using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc5SiteMapBuilder.Providers
{
    public interface IDynamicNodeProvider
    {
        IEnumerable<SiteMapNode> GetSiteMapNodes();
    }
}
