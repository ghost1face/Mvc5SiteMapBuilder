using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvc5SiteMapBuilder.Cache;

namespace Mvc5SiteMapBuilder
{
    public interface ISiteMapBuilderSetStrategy
    {
        ISiteMapBuilderSet GetBuilderSet(string builderSetName);
        ICacheDetails GetCacheDetails(string builderSetName);
    }
}
