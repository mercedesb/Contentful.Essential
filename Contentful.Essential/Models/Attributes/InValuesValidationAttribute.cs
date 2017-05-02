using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;

namespace Contentful.Essential.Models.Attributes
{
	/// <summary>
	/// Attribute to apply in validation to the field. This is only valid on field types 'Sybmol', 'Text', 'Integer', and 'Number'
	/// In validation validates that the field value is in this array.
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
	public class InValuesValidationAttribute : Attribute, IHaveFieldValidation
	{
		public InValuesValidationAttribute(object[] allowedValues, string message = "")
		{
			List<string> requiredValues = new List<string>();
			foreach (var value in allowedValues)
			{
				requiredValues.Add(value.ToString());
			}
			Validator = new InValuesValidator(requiredValues, message);
		}

		public InValuesValidationAttribute(int[] allowedValues, string message = "")
		{
			List<string> requiredValues = new List<string>();
			foreach (var value in allowedValues)
			{
				requiredValues.Add(value.ToString());
			}
			Validator = new InValuesValidator(requiredValues, message);
		}

		public InValuesValidationAttribute(long[] allowedValues, string message = "")
		{
			List<string> requiredValues = new List<string>();
			foreach (var value in allowedValues)
			{
				requiredValues.Add(value.ToString());
			}
			Validator = new InValuesValidator(requiredValues, message);
		}

		public InValuesValidationAttribute(double[] allowedValues, string message = "")
		{
			List<string> requiredValues = new List<string>();
			foreach (var value in allowedValues)
			{
				requiredValues.Add(value.ToString());
			}
			Validator = new InValuesValidator(requiredValues, message);
		}

		public InValuesValidationAttribute(decimal[] allowedValues, string message = "")
		{
			List<string> requiredValues = new List<string>();
			foreach (var value in allowedValues)
			{
				requiredValues.Add(value.ToString());
			}
			Validator = new InValuesValidator(requiredValues, message);
		}

		public IFieldValidator Validator { get; protected set; }
		public string[] ValidFieldTypes
		{
			get
			{
				return new[] { SystemFieldTypes.Symbol, SystemFieldTypes.Text, SystemFieldTypes.Integer, SystemFieldTypes.Number };
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