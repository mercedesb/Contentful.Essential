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
    public class BaseCachedContentRepository<T> : IContentRepository<T>, ICachedContentRepository
          where T : class, IContentType
    {
        protected const string CACHE_KEY = "Entries";
        protected const string CTS_KEY = "Cts";

        private readonly IContentRepository<T> _repo;
        private readonly IMemoryCache _cache;

        public BaseCachedContentRepository(IMemoryCache memoryCache)
        {
            _repo = new BaseContentRepository<T>();
            _cache = memoryCache;
        }

        public virtual async Task<T> Get(string id)
        {
            string cacheKey = GetCacheKey(id);
            string ctsKey = GetCancellationTokenSourceCacheKey(cacheKey);
            var cts = new CancellationTokenSource();
            _cache.Set(ctsKey, cts);

            T result;
            if (!_cache.TryGetValue(cacheKey, out result))
            {
                result = await _repo.Get(id);
                if (result != null)
                {
                    MemoryCacheEntryOptions opts = new MemoryCacheEntryOptions().RegisterPostEvictionCallback(
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
                    CancellationTokenSource cts = _cache.Get<CancellationTokenSource>(GetCancellationTokenSourceCacheKey(cacheKey));
                    MemoryCacheEntryOptions opts = new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(cts.Token));
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
                    CancellationTokenSource cts = _cache.Get<CancellationTokenSource>(GetCancellationTokenSourceCacheKey(cacheKey));
                    MemoryCacheEntryOptions opts = new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(cts.Token));
                    _cache.Set(cacheKey, result, opts);
                }
            }

            return result;
        }

        public virtual void PurgeCache(string id)
        {
            string cacheKey = GetCacheKey(id);
            _cache.Remove(cacheKey);
        }

        public virtual void PurgeCache()
        {
            string cacheKey = GetCacheKey();
            _cache.Remove(cacheKey);
        }

        protected virtual string GetCacheKey()
        {
            return $"{CACHE_KEY}_{typeof(T).Name}";
        }

        protected virtual string GetCacheKey(string id)
        {
            return $"{CACHE_KEY}_{id}";
        }

        protected virtual string GetCacheKey(QueryBuilder<Entry<T>> builder)
        {
            return $"{CACHE_KEY}_{typeof(T).Name}_{builder.Build()}";
        }

        protected virtual string GetCancellationTokenSourceCacheKey(string entryKey)
        {
            return $"{CTS_KEY}_{entryKey}";
        }
    }
}
