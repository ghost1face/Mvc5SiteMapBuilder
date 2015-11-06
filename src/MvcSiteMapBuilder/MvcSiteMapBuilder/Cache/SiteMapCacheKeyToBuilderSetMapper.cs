using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc5SiteMapBuilder.Cache
{
    public class SiteMapCacheKeyToBuilderSetMapper : ISiteMapCacheKeyToBuilderSetMapper
    {
        public virtual string GetBuilderSetName(string cacheKey)
        {
            return "default";
        }
    }
}
