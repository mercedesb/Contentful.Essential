using Contentful.Essential.Models.Management;
using Contentful.Core.Models.Management;
using System;

namespace Contentful.Essential.Models.Attributes
{
	/// <summary>
	/// Attribute to apply image size validation to the field. This is only valid on field types 'Link' and link type 'Asset'
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
	public class ImageSizeValidationAttribute : Attribute, IHaveFieldValidation
	{
		protected int? minWidth = null;
		public int MinWidth
		{
			get
			{
				return this.minWidth.GetValueOrDefault();
			}
			set
			{
				this.minWidth = value;
			}
		}

		protected int? maxWidth = null;
		public int MaxWidth
		{
			get
			{
				return this.maxWidth.GetValueOrDefault();
			}
			set
			{
				this.maxWidth = value;
			}
		}

		protected int? minHeight = null;
		public int MinHeight
		{
			get
			{
				return this.minHeight.GetValueOrDefault();
			}
			set
			{
				this.minHeight = value;
			}
		}

		protected int? maxHeight = null;
		public int MaxHeight
		{
			get
			{
				return this.maxHeight.GetValueOrDefault();
			}
			set
			{
				this.maxHeight = value;
			}
		}
		public string Message { get; set; }


		public IFieldValidator Validator
		{
			get
			{
				return new ImageSizeValidator(minWidth, maxWidth, minHeight, maxHeight, Message);
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