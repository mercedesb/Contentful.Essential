using Contentful.Core.Models;
using Contentful.Essential.Models.Attributes;
using System;

namespace Contentful.Essential.Models
{
	public abstract class BaseEntry : IEntry
	{
		protected ContentTypeDefinitionAttribute _contentTypeDef;
		protected virtual ContentTypeDefinitionAttribute ContentTypeDefinition
		{
			get
			{
				if (_contentTypeDef == null)
					_contentTypeDef = (ContentTypeDefinitionAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(ContentTypeDefinitionAttribute));

				return _contentTypeDef;
			}
		}
		public SystemProperties Sys { get; set; }

		public string ContentTypeId
		{
			get
			{
				if (Sys != null)
					return Sys.ContentType.SystemProperties.Id;
				else if (ContentTypeDefinition != null)
					return ContentTypeDefinition.ContentTypeId;

				return string.Empty;
			}
		}

		public string ContentTypeName
		{
			get
			{
				if (Sys != null)
					return Sys.ContentType.Name;
				else if (ContentTypeDefinition != null)
					return ContentTypeDefinition.Name;
				else
					return string.Empty;
			}
		}

		public string ContentTypeDescription
		{
			get
			{
				if (Sys != null)
					return Sys.ContentType.Description;
				else if (ContentTypeDefinition != null)
					return ContentTypeDefinition.Description;
				else
					return string.Empty;
			}
		}
	}
}