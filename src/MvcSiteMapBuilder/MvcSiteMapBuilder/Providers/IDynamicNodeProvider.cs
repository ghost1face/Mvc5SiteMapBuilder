using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcSiteMapBuilder.Providers
{
    public interface IDynamicNodeProvider
    {
        IEnumerable<SiteMapNode> GetSiteMapNodes();
    }
}
