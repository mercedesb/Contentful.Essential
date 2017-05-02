using Contentful.Core.Models;

namespace Contentful.Essential.Models
{
	public interface IEntry
	{
		SystemProperties Sys { get; set; }
		string ContentTypeId { get; }
		string ContentTypeName { get; }
		string ContentTypeDescription { get; }
	}
}