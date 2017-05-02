using Contentful.Core.Search;
using Contentful.Essential.Http;
using Contentful.Essential.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Essential.Models
{
	public class BaseContentRepository<T> : IRepository<T> where T : BaseEntry
	{
		public virtual async Task<T> Get(string id)
		{
			ContentTypeDefinitionAttribute contentTypeIdAttr = (ContentTypeDefinitionAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(ContentTypeDefinitionAttribute));
			if (contentTypeIdAttr != null)
			{
				var builder = new QueryBuilder<T>().ContentTypeIs(contentTypeIdAttr.ContentTypeId).FieldEquals(f => f.Sys.Id, id);
				// need to use GetEntries b/c including referenced content is only supported for the methods that return collections. 
				T entry = (await ContentDelivery.Instance.GetEntriesAsync<T>(builder)).FirstOrDefault();
				return entry;
			}
			return null;
		}

		public virtual async Task<IEnumerable<T>> GetAll()
		{
			ContentTypeDefinitionAttribute contentTypeIdAttr = (ContentTypeDefinitionAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(ContentTypeDefinitionAttribute));
			if (contentTypeIdAttr != null)
			{
				var builder = new QueryBuilder<T>().ContentTypeIs(contentTypeIdAttr.ContentTypeId);
				IEnumerable<T> entries = await ContentDelivery.Instance.GetEntriesAsync<T>(builder);
				return entries;
			}
			return Enumerable.Empty<T>();
		}
	}
}