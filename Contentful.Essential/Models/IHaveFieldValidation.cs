using Contentful.Core.Models.Management;

namespace Contentful.Essential.Models
{
	public interface IHaveFieldValidation
	{
		IFieldValidator Validator { get; }
		string[] ValidFieldTypes { get; }
		FieldLinkType[] ValidLinkTypes { get; }
	}
}