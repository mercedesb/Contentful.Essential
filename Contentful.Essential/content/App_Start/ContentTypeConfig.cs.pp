using Contentful.CodeFirst;
using Contentful.Essential.Configuration;
using Contentful.Essential.Models;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Contentful.Essential.Application
{
    public class ContentTypeConfig
    {
        public static void RegisterContentTypes()
        {
            IEnumerable<IContentType> registeredContentTypes = ServiceLocator.Current.GetAllInstances<IContentType>();
            var types = registeredContentTypes.Select(ct => ct.GetType()).Where(c => c.GetTypeInfo().IsClass && c.GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>() != null);
            var contentTypesToCreate = ContentTypeBuilder.InitializeContentTypes(types);
            var createdContentTypes = ContentTypeBuilder.CreateContentTypes(contentTypesToCreate, GetConfig(), ContentManagement.Instance).Result;
        }

        private static ContentfulCodeFirstConfiguration GetConfig()
        {
            return new ContentfulCodeFirstConfiguration
            {
                ApiKey = ConfigurationManager.Instance.ContentfulOptions.ManagementApiKey,
                SpaceId = ConfigurationManager.Instance.ContentfulOptions.SpaceId,
                ForceUpdateContentTypes = true,
                PublishAutomatically = true
            };
        }
    }
}