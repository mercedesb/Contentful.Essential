namespace Contentful.Essential.Configuration
{
    public interface IContentfulOptions
    {
        string DeliveryAPIKey { get; set; }
        string ManagementAPIKey { get; set; }
        string SpaceID { get; set; }
        bool UsePreviewAPI { get; set; }
        int MaxNumberOfRateLimitRetries { get; set; }
    }

	public class ContentfulOptions : IContentfulOptions 
	{
		public string DeliveryAPIKey { get; set; }

		public string ManagementAPIKey { get; set; }

		public string SpaceID { get; set; }

		public bool UsePreviewAPI { get; set; }

		public int MaxNumberOfRateLimitRetries { get; set; }
	}
}