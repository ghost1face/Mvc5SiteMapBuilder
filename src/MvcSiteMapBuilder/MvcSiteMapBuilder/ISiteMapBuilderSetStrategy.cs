using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcSiteMapBuilder.Cache;

namespace MvcSiteMapBuilder
{
    public interface ISiteMapBuilderSetStrategy
    {
        ISiteMapBuilderSet GetBuilderSet(string builderSetName);
        ICacheDetails GetCacheDetails(string builderSetName);
    }
}
