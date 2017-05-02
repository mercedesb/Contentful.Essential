using Contentful.Core;
using Contentful.Essential.Configuration;
using System.Net.Http;

namespace Contentful.Essential.Http
{
	public sealed class ContentDelivery
	{
		private static readonly ContentDelivery instance = new ContentDelivery();
		private static ContentfulClient client = new ContentfulClient(new HttpClient(), ConfigurationManager.Instance.ContentfulOptions);
		private ContentDelivery() { }

		public static ContentfulClient Instance { get { return client; } }
	}

	public sealed class ContentManagement
	{
		private static readonly ContentManagement instance = new ContentManagement();
		private static readonly ContentfulManagementClient client = new ContentfulManagementClient(new HttpClient(), ConfigurationManager.Instance.ContentfulOptions);

		private ContentManagement() { }

		public static ContentfulManagementClient Instance { get { return client; } }
	}
}