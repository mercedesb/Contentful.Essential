using Contentful.CodeFirst;
using Contentful.Core.Models;
using Contentful.Core.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Contentful.Essential.Models
{
    public class BaseContentRepository<T> : IContentRepository<T>
        where T : class, IContentType
    {
        protected readonly IContentDeliveryClient _deliveryClient;
        protected readonly ILogger<BaseContentRepository<T>> _logger;

        //public BaseContentRepository(IContentDeliveryClient deliveryClient, ILogger<BaseContentRepository<T>> logger)
        public BaseContentRepository(IContentDeliveryClient deliveryClient)
        {
            _deliveryClient = deliveryClient;
            //_logger = logger;
        }

        public virtual async Task<T> Get(string id)
        {
            ContentTypeAttribute contentTypeIdAttr = typeof(T).GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>() ?? new ContentTypeAttribute();
            var builder = new QueryBuilder<T>().ContentTypeIs(contentTypeIdAttr.Id ?? typeof(T).FullName).FieldEquals(f => f.Sys.Id, id);
            // need to use GetEntries b/c including referenced content is only supported for the methods that return collections. 
            try
            {
                T entry = (await _deliveryClient.Instance.GetEntriesAsync<T>(builder)).FirstOrDefault();
                return entry;
            }
            catch (Exception ex)
            {
                //_logger.LogError(LoggingEvents.CDAError, ex, $"Unable to get entry {id} for type {typeof(T).Name}");
            }
            return null;
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            ContentTypeAttribute contentTypeIdAttr = typeof(T).GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>() ?? new ContentTypeAttribute();
            var builder = new QueryBuilder<T>().ContentTypeIs(contentTypeIdAttr.Id ?? typeof(T).FullName);
            try
            {
                IEnumerable<T> entries = await _deliveryClient.Instance.GetEntriesAsync<T>(builder);
                return entries;
            }
            catch (Exception ex)
            {
                //_logger.LogError(LoggingEvents.CDAError, ex, $"Unable to get all entries for type {typeof(T).Name}");
            }
            return Enumerable.Empty<T>();
        }

        public virtual async Task<IEnumerable<T>> Search(QueryBuilder<T> builder)
        {
            ContentTypeAttribute contentTypeIdAttr = typeof(T).GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>() ?? new ContentTypeAttribute();
            builder = builder.ContentTypeIs(contentTypeIdAttr.Id ?? typeof(T).FullName);
            try
            {
                IEnumerable<T> entries = await _deliveryClient.Instance.GetEntriesAsync<T>(builder);
                return entries;
            }
            catch (Exception ex)
            {
                //_logger.LogError(LoggingEvents.CDAError, ex, $"Unable to get entries matching criteria {builder.Build()} for type {typeof(T).Name}");
            }
            return Enumerable.Empty<T>();
        }
    }
}