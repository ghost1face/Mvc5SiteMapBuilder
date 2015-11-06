using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc5SiteMapBuilder.Cache
{
    public interface ICacheDependency
    {
        object Dependency { get; }
    }
}
