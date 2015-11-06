using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc5SiteMapBuilder.Cache
{
    /// <summary>
    /// The default mapper class the simply maps everything to the default <see cref="ISiteMapBuilderS"/>
    /// </summary>
    public interface ISiteMapCacheKeyToBuilderSetMapper
    {
        string GetBuilderSetName(string cacheKey);
    }
}
