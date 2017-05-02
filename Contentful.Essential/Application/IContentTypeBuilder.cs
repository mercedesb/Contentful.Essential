using System.Threading.Tasks;

namespace Contentful.Essential.Application
{
	public interface IContentTypeBuilder
	{
		Task CreateOrUpdateContentTypes();
	}
}