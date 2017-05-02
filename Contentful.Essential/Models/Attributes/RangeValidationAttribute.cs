using Contentful.Core.Models.Management;
using System;

namespace Contentful.Essential.Models.Attributes
{
	/// <summary>
	/// Attribute to apply mime type validation to the field. This is only valid on field types 'Integer' and 'Number'
	/// Range validation validates the range of a value.
	///  To specify null for the min or max value, use -1
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
	public class RangeValidationAttribute : Attribute, IHaveFieldValidation
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
		public string Message { get; set; }

		public IFieldValidator Validator
		{
			get
			{
				return new RangeValidator(min, max, Message);
			}
		}
		public string[] ValidFieldTypes
		{
			get
			{
				return new[] { SystemFieldTypes.Integer, SystemFieldTypes.Number };
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