using Contentful.CodeFirst;
using Contentful.Core.Models;
using Contentful.Core.Search;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Contentful.Essential.Models
{
    public class BaseContentRepository<T> : IContentRepository<T>
        where T : class, IContentType
    {
        protected readonly IContentDeliveryClient _deliveryClient;
        public BaseContentRepository(IContentDeliveryClient deliveryClient)
        {
            _deliveryClient = deliveryClient;
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
                SystemLog.Log(this, $"Unable to get entry {id} for type {typeof(T).Name}", Level.Error, ex);
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
                SystemLog.Log(this, $"Unable to get all entries for type {typeof(T).Name}", Level.Error, ex);
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
                SystemLog.Log(this, $"Unable to get entries matching criteria {builder.Build()} for type {typeof(T).Name}", Level.Error, ex);
            }
            return Enumerable.Empty<T>();
        }
    }
}