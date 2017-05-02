using Contentful.Core.Models.Management;
using System;

namespace Contentful.Essential.Models.Attributes
{
	/// <summary>
	/// Attribute to apply mime type validation to the field. This is only valid on field types 'Symbol' and 'Text'
	/// Mime type validation validates the JS regex against a string.
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
	public class RegexValidationAttribute : Attribute, IHaveFieldValidation
	{
		public string Expression { get; set; }
		public string Flags { get; set; }
		public string Message { get; set; }

		public IFieldValidator Validator
		{
			get
			{
				return new RegexValidator(Expression, Flags, Message);
			}
		}
		public string[] ValidFieldTypes
		{
			get
			{
				return new[] { SystemFieldTypes.Symbol, SystemFieldTypes.Text };
			}
		}
		public FieldLinkType[] ValidLinkTypes
		{
			get
			{
				return new FieldLinkType[0];
			}
		}
	}
}