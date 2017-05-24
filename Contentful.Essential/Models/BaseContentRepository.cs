using Contentful.CodeFirst;
using Contentful.Core.Models;
using Contentful.Core.Search;
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
        public virtual async Task<T> Get(string id)
        {
            ContentTypeAttribute contentTypeIdAttr = typeof(T).GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>();
            if (contentTypeIdAttr != null)
            {
                var builder = new QueryBuilder<T>().ContentTypeIs(contentTypeIdAttr.Id ?? typeof(T).FullName).FieldEquals(f => f.Sys.Id, id);
                // need to use GetEntries b/c including referenced content is only supported for the methods that return collections. 
                T entry = (await ContentDelivery.Instance.GetEntriesAsync<T>(builder)).FirstOrDefault();
                return entry;
            }
            return null;
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            ContentTypeAttribute contentTypeIdAttr = typeof(T).GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>();
            if (contentTypeIdAttr != null)
            {
                var builder = new QueryBuilder<T>().ContentTypeIs(contentTypeIdAttr.Id ?? typeof(T).FullName);
                IEnumerable<T> entries = await ContentDelivery.Instance.GetEntriesAsync<T>(builder);
                return entries;
            }
            return Enumerable.Empty<T>();
        }

        public virtual async Task<IEnumerable<T>> Search(QueryBuilder<T> builder)
        {
            IEnumerable<T> entries = await ContentDelivery.Instance.GetEntriesAsync<T>(builder);
            return entries;
        }
    }
}