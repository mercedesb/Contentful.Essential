using Contentful.Core.Models;
using Contentful.Core.Search;
using Contentful.Essential.Http;
using Contentful.Essential.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Essential.Models
{
    public class BaseContentRepository<T> : IContentRepository<T>
        where T : class, IContentType
    {
        public virtual async Task<Entry<T>> Get(string id)
        {
            ContentTypeDefinitionAttribute contentTypeIdAttr = (ContentTypeDefinitionAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(ContentTypeDefinitionAttribute));
            if (contentTypeIdAttr != null)
            {
                var builder = new QueryBuilder<Entry<T>>().ContentTypeIs(contentTypeIdAttr.ContentTypeId).FieldEquals(f => f.SystemProperties.Id, id);
                // need to use GetEntries b/c including referenced content is only supported for the methods that return collections. 
                Entry<T> entry = (await ContentDelivery.Instance.GetEntriesAsync<Entry<T>>(builder)).FirstOrDefault();
                return entry;
            }
            return null;
        }

        public virtual async Task<IEnumerable<Entry<T>>> GetAll()
        {
            ContentTypeDefinitionAttribute contentTypeIdAttr = (ContentTypeDefinitionAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(ContentTypeDefinitionAttribute));
            if (contentTypeIdAttr != null)
            {
                var builder = new QueryBuilder<Entry<T>>().ContentTypeIs(contentTypeIdAttr.ContentTypeId);
                IEnumerable<Entry<T>> entries = await ContentDelivery.Instance.GetEntriesAsync<Entry<T>>(builder);
                return entries;
            }
            return Enumerable.Empty<Entry<T>>();
        }
    }
}