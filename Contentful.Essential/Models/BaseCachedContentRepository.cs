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
    public class BaseCachedContentRepository<T> : BaseCachedContentRepositoryPurger, IContentRepository<T>
          where T : class, IContentType
    {
        protected readonly IContentRepository<T> _repo;

        public BaseCachedContentRepository(IContentDeliveryClient deliveryClient, IMemoryCache memoryCache) : base(memoryCache)
        {
            _repo = new BaseContentRepository<T>(deliveryClient);
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
                result = await _repo.Search(builder);
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
