using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using System;

namespace Contentful.Essential.Models.Attributes
{
	/// <summary>
	/// Attribute to apply mime type validation to the field. This is only valid on field types 'Link' with link type 'Asset'
	/// Mime type validation validates that the link points to an asset of this group.
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
	public class MimeTypeValidationAttribute : Attribute, IHaveFieldValidation
	{
		public MimeTypeRestriction[] AllowedTypes { get; set; }
		public string Message { get; set; }


		public IFieldValidator Validator
		{
			get
			{
				return new Contentful.Essential.Models.Management.MimeTypeValidator(AllowedTypes, Message);
			}
		}
		public string[] ValidFieldTypes
		{
			get
			{
				return new[] { SystemFieldTypes.Link };
			}
		}
		public FieldLinkType[] ValidLinkTypes
		{
			get
			{
				return new[] { FieldLinkType.Asset };
			}
		}
	}
}