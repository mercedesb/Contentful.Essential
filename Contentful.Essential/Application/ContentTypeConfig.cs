using Microsoft.Practices.ServiceLocation;

namespace Contentful.Essential.Application
{
	public class ContentTypeConfig
	{
		public static void RegisterContentTypes()
		{
			ServiceLocator.Current.GetInstance<IContentTypeBuilder>().CreateOrUpdateContentTypes();
		}
	}
}