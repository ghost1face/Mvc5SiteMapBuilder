using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading;

namespace Mvc5SiteMapBuilder.Cache
{
    public class InMemoryCacheProvider<T> : ICacheProvider<T>, IDisposable
        where T : class
    {
        private ObjectCache cache = MemoryCache.Default;
        private readonly ReaderWriterLockSlim synclock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private bool disposed;

        public void Add(string key, T item, ICacheDetails cacheDetails)
        {
            var policy = new CacheItemPolicy
            {
                // Setting priority to not removable ensures an 
                // app pool recycle doesn't unload the item, but a timeout will.
                Priority = CacheItemPriority.NotRemovable
            };

            if (IsTimespanSet(cacheDetails.AbsoluteCacheExpiration))
            {
                policy.AbsoluteExpiration = DateTimeOffset.Now.Add(cacheDetails.AbsoluteCacheExpiration);
            }
            else if (IsTimespanSet(cacheDetails.SlidingCacheExpiration))
            {
                policy.SlidingExpiration = cacheDetails.SlidingCacheExpiration;
            }

            // add dependencies
            var dependencies = (IList<ChangeMonitor>)cacheDetails.CacheDependency.Dependency;
            if (dependencies != null)
            {
                foreach (var dependency in dependencies)
                {
                    policy.ChangeMonitors.Add(dependency);
                }
            }

            ExecuteInWriteLock(() => cache.Add(key, item, policy));
        }

        public bool Contains(string key)
        {
            return ExecuteInReadLock(() => cache.Contains(key));
        }

        public T Get(string key)
        {
            return ExecuteInReadLock(() => cache.Get(key) as T);
        }

        public void Remove(string key)
        {
            ExecuteInWriteLock(() => cache.Remove(key));
        }

        public bool TryGetValue(string key, out T item)
        {
            item = Get(key);
            if (item != null)
            {
                return true;
            }
            return false;
        }

        #region Private Methods

        private bool IsTimespanSet(TimeSpan timeSpan)
        {
            return (!timeSpan.Equals(TimeSpan.MinValue));
        }

        /// <summary>
        /// Enters exclusive write access for writing tocache, executing the provided function against the cache
        /// </summary>
        /// <param name="func"></param>
        private void ExecuteInWriteLock(Action func)
        {
            synclock.EnterWriteLock();

            try
            {
                func();
            }
            finally
            {
                synclock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Enters exclusive write access for writing tocache, executing the provided function against the cache
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private T1 ExecuteInWriteLock<T1>(Func<T1> func)
        {
            synclock.EnterWriteLock();

            try
            {
                return func();
            }
            finally
            {
                synclock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Enters exclusive read access while reading from cache, executing the provided function against the cache
        /// </summary>
        /// <param name="func"></param>
        private void ExecuteInReadLock(Action func)
        {
            synclock.EnterReadLock();

            try
            {
                func();
            }
            finally
            {
                synclock.ExitReadLock();
            }
        }

        /// <summary>
        /// Enters exclusive read access while reading from cache, executing the provided function against the cache
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private T1 ExecuteInReadLock<T1>(Func<T1> func)
        {
            synclock.EnterReadLock();

            try
            {
                return func();
            }
            finally
            {
                synclock.ExitReadLock();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (synclock != null)
                    synclock.Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
