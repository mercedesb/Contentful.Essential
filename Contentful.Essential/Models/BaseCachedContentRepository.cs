using Contentful.Core.Models;
using Contentful.Core.Search;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using System.Threading;

namespace Contentful.Essential.Models
{
    public class BaseCachedContentRepositoryPurger : IPurgeCachedContentRepository
    {
        protected const string CACHE_KEY = "Entries";
        protected const string CTS_KEY = "Cts";

        protected readonly IMemoryCache _cache;
        public BaseCachedContentRepositoryPurger(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public virtual void PurgeCache(string id)
        {
            string cacheKey = GetCacheKey(id);
            if (_cache.TryGetValue(cacheKey, out object val))
            {
                _cache.Remove(cacheKey);
            }
            else
            {
                string ctsKey = GetCancellationTokenSourceCacheKey(GetCacheKey(id));
                CancellationTokenSource cts = _cache.Get<CancellationTokenSource>(ctsKey);
                if (cts != null)
                    cts.Cancel();
            }
        }

        protected virtual string GetCacheKey(string id)
        {
            return $"{CACHE_KEY}_{id}";
        }

        protected virtual string GetCancellationTokenSourceCacheKey(string entryKey)
        {
            return $"{CTS_KEY}_{entryKey}";
        }
    }

    public class BaseCachedContentRepository<T> : BaseCachedContentRepositoryPurger, IContentRepository<T>
          where T : class, IContentType
    {
        protected readonly IContentRepository<T> _repo;

        public BaseCachedContentRepository(IMemoryCache memoryCache) : base(memoryCache)
        {
            _repo = new BaseContentRepository<T>();
        }

        public virtual async Task<T> Get(string id)
        {
            string cacheKey = GetCacheKey(id);
            CancellationTokenSource cts = GetCancellationTokenSource(id);

            T result;
            if (!_cache.TryGetValue(cacheKey, out result))
            {
                result = await _repo.Get(id);
                if (result != null)
                {
                    MemoryCacheEntryOptions opts = new MemoryCacheEntryOptions();
                    opts.SetSlidingExpiration(new System.TimeSpan(7, 0, 0, 0));
                    opts.RegisterPostEvictionCallback(
                        (key, value, reason, substate) => {
                            _cache.Get<CancellationTokenSource>(GetCancellationTokenSourceCacheKey(key.ToString())).Cancel();
                        });

                    _cache.Set(cacheKey, result, opts);
                }
            }
            return result;
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            string cacheKey = GetCacheKey();

            IEnumerable<T> result;
            if (!_cache.TryGetValue(cacheKey, out result))
            {
                result = await _repo.GetAll();
                if (result != null)
                {
                    MemoryCacheEntryOptions opts = new MemoryCacheEntryOptions();
                    opts.SetSlidingExpiration(new System.TimeSpan(7, 0, 0, 0));
                    foreach (T item in result)
                    {
                        CancellationTokenSource cts = GetCancellationTokenSource(item.Sys.Id);
                        opts.AddExpirationToken(new CancellationChangeToken(cts.Token));
                    }
                    _cache.Set(cacheKey, result, opts);
                }
            }

            return result;
        }

        public virtual async Task<IEnumerable<T>> Search(QueryBuilder<T> builder)
        {
            string cacheKey = GetCacheKey();

            IEnumerable<T> result;
            if (!_cache.TryGetValue(cacheKey, out result))
            {
                result = await ContentDelivery.Instance.GetEntriesAsync<T>(builder);
                if (result != null)
                {
                    MemoryCacheEntryOptions opts = new MemoryCacheEntryOptions();
                    opts.SetSlidingExpiration(new System.TimeSpan(7, 0, 0, 0));
                    foreach (T item in result)
                    {
                        CancellationTokenSource cts = GetCancellationTokenSource(item.Sys.Id);
                        opts.AddExpirationToken(new CancellationChangeToken(cts.Token));
                    }
                    _cache.Set(cacheKey, result, opts);
                }
            }

            return result;
        }

        protected virtual string GetCacheKey()
        {
            return $"{CACHE_KEY}_{typeof(T).Name}";
        }

        protected virtual string GetCacheKey(QueryBuilder<Entry<T>> builder)
        {
            return $"{CACHE_KEY}_{typeof(T).Name}_{builder.Build()}";
        }

        protected virtual CancellationTokenSource GetCancellationTokenSource(string id)
        {
            string ctsKey = GetCancellationTokenSourceCacheKey(GetCacheKey(id));
            CancellationTokenSource cts = _cache.Get<CancellationTokenSource>(ctsKey);
            if (cts == null)
            {
                cts = new CancellationTokenSource();
                _cache.Set(ctsKey, cts);
            }
            return cts;
        }
    }
}
