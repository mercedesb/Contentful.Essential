using Contentful.CodeFirst;
using Contentful.Essential.Configuration;
using System.Threading.Tasks;

namespace Contentful.Essential.Application
{
    public class ContentTypeConfig
    {
        public static async Task RegisterContentTypes()
        {
            // TODO: control which assembly is scanned
            await ContentTypeBuilder.CreateContentTypesFromAssembly(typeof(ContentTypeConfig).Assembly.FullName, GetConfig());

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