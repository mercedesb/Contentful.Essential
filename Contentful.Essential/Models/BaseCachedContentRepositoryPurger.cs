using Microsoft.Extensions.Caching.Memory;
using System.Threading;

<<<<<<< HEAD
=======

>>>>>>> master
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
<<<<<<< HEAD
}
=======
}
>>>>>>> master
