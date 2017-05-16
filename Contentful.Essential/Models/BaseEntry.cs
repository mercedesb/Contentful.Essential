using Contentful.CodeFirst;
using Contentful.Core.Models;
using System;

namespace Contentful.Essential.Models
{
    public abstract class BaseEntry : IContentType
    {
        protected ContentTypeAttribute _contentTypeDef;
        protected virtual ContentTypeAttribute ContentTypeDefinition
        {
            get
            {
                if (_contentTypeDef == null)
                    _contentTypeDef = (ContentTypeAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(ContentTypeAttribute));

                return _contentTypeDef;
            }
        }
        [IgnoreContentField]
        public SystemProperties Sys { get; set; }

        public string ContentTypeId
        {
            get
            {
                if (Sys != null)
                    return Sys.ContentType.SystemProperties.Id;
                else if (ContentTypeDefinition != null)
                    return ContentTypeDefinition.Id ?? this.GetType().Name;

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
                    return ContentTypeDefinition.Name ?? this.GetType().FullName;
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