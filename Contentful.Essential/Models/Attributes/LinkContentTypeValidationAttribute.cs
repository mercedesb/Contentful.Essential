using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contentful.Essential.Models.Attributes
{
    /// <summary>
    /// Attribute to apply link content type validation to the field. This is only valid on field types 'Link' with link type 'Entry.'
    /// Link content type validation validates that the link points to an entry of that content type.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class LinkContentTypeValidationAttribute : Attribute, IHaveFieldValidation
    {
        public Type[] AllowedTypes { get; set; }
        public string Message { get; set; }


        public IFieldValidator Validator
        {
            get
            {
                List<string> contentTypeIds = new List<string>();
                if (AllowedTypes == null)
                    return new LinkContentTypeValidator(contentTypeIds, Message);

                foreach (var type in AllowedTypes)
                {
                    if (typeof(IContentType).IsAssignableFrom(type))
                    {
                        ContentTypeDefinitionAttribute contentTypeDef = (ContentTypeDefinitionAttribute)Attribute.GetCustomAttribute(type, typeof(ContentTypeDefinitionAttribute));
                        if (contentTypeDef != null)
                            contentTypeIds.Add(contentTypeDef.ContentTypeId);

                        IEnumerable<Type> childTypes = type.Assembly.GetTypes().Where(t => t != type && type.IsAssignableFrom(t));
                        foreach (var subClass in childTypes)
                        {
                            contentTypeDef = (ContentTypeDefinitionAttribute)Attribute.GetCustomAttribute(subClass, typeof(ContentTypeDefinitionAttribute));
                            if (contentTypeDef != null)
                                contentTypeIds.Add(contentTypeDef.ContentTypeId);
                        }
                    }
                }
                if (contentTypeIds.Any())
                    return new LinkContentTypeValidator(contentTypeIds.Distinct(), Message);

                return null;
            }
        }

        public string[] ValidFieldTypes
        {
            get
            {
                return new[] { SystemFieldTypes.Link };
            }
        }
        public FieldLinkType[] ValidLinkTypes
        {
            get
            {
                return new[] { FieldLinkType.Entry };
            }
        }
    }
}