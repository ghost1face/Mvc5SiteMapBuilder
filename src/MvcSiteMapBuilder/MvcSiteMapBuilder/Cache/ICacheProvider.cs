namespace MvcSiteMapBuilder.Cache
{
    public interface ICacheProvider<T>
    {
        bool Contains(string key);
        T Get(string key);
        bool TryGetValue(string key, out T item);
        void Add(string key, T item, ICacheDetails cacheDetails);
        void Remove(string key);
    }
}
