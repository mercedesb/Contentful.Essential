using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Contentful.Essential.Models.Management
{
	public class FieldValidatorContractResolver : DefaultContractResolver
	{
		protected override JsonConverter ResolveContractConverter(Type objectType)
		{
			if (typeof(IFieldValidator).IsAssignableFrom(objectType))
				return new CustomValidationsJsonConverter();
			return base.ResolveContractConverter(objectType);
		}
	}
}
