using Contentful.Essential.Models.Management;
using Contentful.Core.Models.Management;
using System;

namespace Contentful.Essential.Models.Attributes
{
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
	public class DateRangeValidationAttribute : Attribute, IHaveFieldValidation
	{
		/// <summary>
		/// The minimum date (yyyy-MM-dd)
		/// </summary>
		public string Min { get; set; }

		/// <summary>
		/// The maximum date (yyyy-MM-dd)
		/// </summary>
		public string Max { get; set; }
		public string Message { get; set; }

		public IFieldValidator Validator
		{
			get
			{
				return new DateRangeValidator(Min, Max, Message);
			}
		}
		public string[] ValidFieldTypes
		{
			get
			{
				return new[] { SystemFieldTypes.Date };
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