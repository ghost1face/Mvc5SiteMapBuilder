
namespace MvcSiteMapBuilder.Cache
{
    public class NullCacheDependency : ICacheDependency
    {
        public object Dependency
        {
            get { return null; }
        }
    }
}
