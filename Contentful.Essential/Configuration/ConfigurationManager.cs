using Contentful.Core.Configuration;

namespace Contentful.Essential.Configuration
{
	public class ConfigurationManager
	{
		private static readonly ConfigurationManager instance = new ConfigurationManager();
		private ConfigurationManager() { }

		public static ConfigurationManager Instance { get { return instance; } }

		public ContentfulOptions ContentfulOptions
		{
			get
			{
				ContentfulEssentialSection configuredOptions = (ContentfulEssentialSection)System.Configuration.ConfigurationManager.GetSection("contentful.essential");
				return new ContentfulOptions
				{
					DeliveryApiKey = configuredOptions.ContentfulOptions.DeliveryAPIKey,
					ManagementApiKey = configuredOptions.ContentfulOptions.ManagementAPIKey,
					SpaceId = configuredOptions.ContentfulOptions.SpaceID,
					UsePreviewApi = configuredOptions.ContentfulOptions.UsePreviewAPI,
					MaxNumberOfRateLimitRetries = configuredOptions.ContentfulOptions.MaxNumberOfRateLimitRetries
				};
			}
		}
	}
}