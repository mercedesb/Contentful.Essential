﻿using Contentful.Core.Models;
using Contentful.Core.Search;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Contentful.Essential.Models
{
    public class BaseCachedContentRepository<T> : IContentRepository<T>, ICachedContentRepository
          where T : class, IContentType
    {
        protected const string CACHE_KEY = "Entries";

        private readonly IContentRepository<T> _repo;

        public BaseCachedContentRepository()
        {
            _repo = new BaseContentRepository<T>();
        }

        public virtual async Task<Entry<T>> Get(string id)
        {
            string cacheKey = GetCacheKey(id);
            Entry<T> result = MemoryCache.Default[cacheKey] as Entry<T>;
            if (result == null)
            {
                result = await _repo.Get(id);
                if (result != null)
                {
                    CacheItemPolicy policy = new CacheItemPolicy();
                    MemoryCache.Default.Add(cacheKey, result, policy);
                }
            }
            return result;
        }

        public virtual async Task<IEnumerable<Entry<T>>> GetAll()
        {
            string cacheKey = GetCacheKey();
            IEnumerable<Entry<T>> result = MemoryCache.Default[cacheKey] as IEnumerable<Entry<T>>;
            if (result == null)
            {
                result = await _repo.GetAll();
                if (result != null)
                {
                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.ChangeMonitors.Add(MemoryCache.Default.CreateCacheEntryChangeMonitor(result.Select(r => GetCacheKey(r.SystemProperties.Id))));
                    MemoryCache.Default.Add(cacheKey, result, policy);
                }
            }

            return result;
        }

        public virtual async Task<IEnumerable<Entry<T>>> Search(QueryBuilder<Entry<T>> builder)
        {
            string cacheKey = GetCacheKey();
            IEnumerable<Entry<T>> result = MemoryCache.Default[cacheKey] as IEnumerable<Entry<T>>;
            if (result == null)
            {
                result = await ContentDelivery.Instance.GetEntriesAsync<Entry<T>>(builder);
                if (result != null)
                {
                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.ChangeMonitors.Add(MemoryCache.Default.CreateCacheEntryChangeMonitor(result.Select(r => GetCacheKey(r.SystemProperties.Id))));
                    MemoryCache.Default.Add(cacheKey, result, policy);
                }
            }

            return result;
        }

        public virtual void PurgeCache(string id)
        {
            string cacheKey = GetCacheKey(id);
            MemoryCache.Default.Remove(cacheKey);
        }

        public virtual void PurgeCache()
        {
            string cacheKey = GetCacheKey();
            MemoryCache.Default.Remove(cacheKey);
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
    }
}