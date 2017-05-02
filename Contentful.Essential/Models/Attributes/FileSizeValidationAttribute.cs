using Contentful.Essential.Models.Management;
using Contentful.Core.Models.Management;
using System;

namespace Contentful.Essential.Models.Attributes
{
	/// <summary>
	/// Attribute to apply file size validation to the field. This is only valid on field types 'Link' and link type 'Asset'
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
	public class FileSizeValidationAttribute : Attribute, IHaveFieldValidation
	{
		protected int? min = null;
		public int Min
		{
			get
			{
				return this.min.GetValueOrDefault();
			}
			set
			{
				this.min = value;
			}
		}

		protected int? max = null;
		public int Max
		{
			get
			{
				return this.max.GetValueOrDefault();
			}
			set
			{
				this.max = value;
			}
		}


		public FileSizeUnit MinUnit { get; set; }
		public FileSizeUnit MaxUnit { get; set; }

		public string Message { get; set; }


		public IFieldValidator Validator
		{
			get
			{
				return new FileSizeValidator(min, max, MinUnit, MaxUnit, Message);
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