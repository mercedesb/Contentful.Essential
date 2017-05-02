using Contentful.Core.Models.Management;
using System;

namespace Contentful.Essential.Models.Attributes
{
	/// <summary>
	/// Attribute to apply mime type validation to the field. This is only valid on field types 'Symbol', 'Integer', and 'Number'
	/// Mime type validation validates that there are no other entries that have the same field value at the time of publication.
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
	public class UniqueValidationAttribute : Attribute, IHaveFieldValidation
	{
		public IFieldValidator Validator
		{
			get
			{
				return new UniqueValidator();
			}
		}
		public string[] ValidFieldTypes
		{
			get
			{
				return new[] { SystemFieldTypes.Symbol, SystemFieldTypes.Integer, SystemFieldTypes.Number };
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