using System.Configuration;

namespace Contentful.Essential.Configuration
{
	// Define the "colorcontentfulOptions" element 
	// with "background" and "foreground" attributes.
	public class ContentfulOptionsElement : ConfigurationElement
	{

		[ConfigurationProperty("deliveryApiKey", IsRequired = true)]
		public string DeliveryAPIKey
		{
			get
			{
				return (string)this["deliveryApiKey"];
			}
			set
			{
				this["deliveryApiKey"] = value;
			}
		}

		[ConfigurationProperty("managementApiKey", IsRequired = true)]
		public string ManagementAPIKey
		{
			get
			{
				return (string)this["managementApiKey"];
			}
			set
			{
				this["managementApiKey"] = value;
			}
		}

		[ConfigurationProperty("spaceId", IsRequired = true)]
		public string SpaceID
		{
			get
			{
				return (string)this["spaceId"];
			}
			set
			{
				this["spaceId"] = value;
			}
		}

		[ConfigurationProperty("usePreviewApi", IsRequired = false, DefaultValue = false)]
		public bool UsePreviewAPI
		{
			get
			{
				return (bool)this["usePreviewApi"];
			}
			set
			{
				this["usePreviewApi"] = value;
			}
		}

		[ConfigurationProperty("maxNumberOfRateLimitRetries", IsRequired = false, DefaultValue = 0)]
		public int MaxNumberOfRateLimitRetries
		{
			get
			{
				return (int)this["maxNumberOfRateLimitRetries"];
			}
			set
			{
				this["maxNumberOfRateLimitRetries"] = value;
			}
		}

	}


}