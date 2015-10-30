using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcSiteMapBuilder
{
    public class SiteMap
    {
        public string CacheKey { get; set; }
        public List<SiteMapNode> Nodes { get; set; }
    }
}
