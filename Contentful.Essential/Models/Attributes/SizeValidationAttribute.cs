using Contentful.Core.Models.Management;
using System;

namespace Contentful.Essential.Models.Attributes
{
	/// <summary>
	/// Attribute to apply size validation to the field. This is only valid on field types 'Sybmol' and 'Text'
	/// Size validation validates the size of the array (number of objects in it).
	/// To specify null for the min or max value, use -1
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
	public class SizeValidationAttribute : Attribute, IHaveFieldValidation
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
				return new SizeValidator(min, max, Message);
			}
		}
		public string[] ValidFieldTypes
		{
			get
			{
				return new[] { SystemFieldTypes.Array, SystemFieldTypes.Symbol, SystemFieldTypes.Text, SystemFieldTypes.Object };
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