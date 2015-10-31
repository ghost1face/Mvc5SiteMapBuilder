namespace MvcSiteMapBuilder.Cache
{
    public class NoCacheProvider<T> : ICacheProvider<T>
        where T : class
    {
        public void Add(string key, T item, ICacheDetails cacheDetails)
        {
            
        }

        public bool Contains(string key)
        {
            return false;
        }

        public T Get(string key)
        {
            return null as T;
        }

        public void Remove(string key)
        {
            
        }

        public bool TryGetValue(string key, out T item)
        {
            item = null;

            return false;
        }
    }
}
