using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Contentful.Essential.Models.Management
{
	public class CustomValidationsJsonConverter : Contentful.Core.Configuration.ValidationsJsonConverter
	{
		protected JObject _currentObject;
		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The reader to use.</param>
		/// <param name="objectType">The object type to serialize into.</param>
		/// <param name="existingValue">The current value of the property.</param>
		/// <param name="serializer">The serializer to use.</param>
		/// <returns>The deserialized <see cref="Asset"/>.</returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			_currentObject = JObject.Load(reader);

			JToken jToken;

			if (_currentObject.TryGetValue("size", out jToken))
			{
				return new SizeValidator(
					jToken["min"] != null && jToken["min"].Type != JTokenType.Null ? new int?(int.Parse(jToken["min"].ToString())) : null,
					jToken["max"] != null && jToken["max"].Type != JTokenType.Null ? new int?(int.Parse(jToken["max"].ToString())) : null,
					_currentObject["message"]?.ToString());
			}

			if (_currentObject.TryGetValue("range", out jToken))
			{
				return new RangeValidator(
					jToken["min"] != null && jToken["min"].Type != JTokenType.Null ? new int?(int.Parse(jToken["min"].ToString())) : null,
					jToken["max"] != null && jToken["max"].Type != JTokenType.Null ? new int?(int.Parse(jToken["max"].ToString())) : null,
					_currentObject["message"]?.ToString());
			}

			if (_currentObject.TryGetValue("in", out jToken))
			{
				return new InValuesValidator(jToken.Values<string>(), _currentObject["message"]?.ToString());
			}

			if (_currentObject.TryGetValue("linkMimetypeGroup", out jToken))
			{
				if (jToken is JValue)
				{
					//single string value returned for mime type field. This seems to be an inconsistency in the API that needs to be handled.

					var type = jToken.Value<string>();
					return new Contentful.Essential.Models.Management.MimeTypeValidator(new[] { (MimeTypeRestriction)Enum.Parse(typeof(MimeTypeRestriction), type, true) },
					_currentObject["message"]?.ToString());
				}

				var types = jToken.Values<string>();
				return new Contentful.Essential.Models.Management.MimeTypeValidator(types.Select(c => (MimeTypeRestriction)Enum.Parse(typeof(MimeTypeRestriction), c, true)),
					_currentObject["message"]?.ToString());
			}

			if (_currentObject.TryGetValue("linkContentType", out jToken))
			{
				return new LinkContentTypeValidator(jToken.Values<string>(), _currentObject["message"]?.ToString());
			}

			if (_currentObject.TryGetValue("regexp", out jToken))
			{
				return new RegexValidator(jToken["pattern"].ToString(), jToken["flags"].ToString(), _currentObject["message"]?.ToString());
			}

			if (_currentObject.TryGetValue("unique", out jToken))
			{
				return new UniqueValidator();
			}

			if (_currentObject.TryGetValue("assetFileSize", out jToken))
			{
				return new FileSizeValidator(
					jToken["min"] != null && jToken["min"].Type != JTokenType.Null ? new int?(int.Parse(jToken["min"].ToString())) : null,
					jToken["max"] != null && jToken["max"].Type != JTokenType.Null ? new int?(int.Parse(jToken["max"].ToString())) : null,
					FileSizeUnit.Byte,
					FileSizeUnit.Byte,
					_currentObject["message"]?.ToString());
			}
			if (_currentObject.TryGetValue("assetImageDimensions", out jToken))
			{
				int? minWidth = null;
				int? maxWidth = null;
				int? minHeight = null;
				int? maxHeight = null;
				if (jToken["width"] != null)
				{
					JToken width = jToken["width"];
					minWidth = width["min"] != null && width["min"].Type != JTokenType.Null ? new int?(int.Parse(width["min"].ToString())) : null;
					maxWidth = width["max"] != null && width["max"].Type != JTokenType.Null ? new int?(int.Parse(width["max"].ToString())) : null;
				}
				if (jToken["height"] != null)
				{
					JToken height = jToken["height"];
					minHeight = height["min"] != null && height["min"].Type != JTokenType.Null ? new int?(int.Parse(height["min"].ToString())) : null;
					maxHeight = height["max"] != null && height["max"].Type != JTokenType.Null ? new int?(int.Parse(height["max"].ToString())) : null;
				}
				return new ImageSizeValidator(minWidth, maxWidth, minHeight, maxHeight, _currentObject["message"]?.ToString());
			}
			if (_currentObject.TryGetValue("dateRange", out jToken))
			{
				return new DateRangeValidator(
					jToken["min"] != null && jToken["min"].Type != JTokenType.Null ? jToken["min"].ToString() : null,
					jToken["max"] != null && jToken["max"].Type != JTokenType.Null ? jToken["max"].ToString() : null,
					_currentObject["message"]?.ToString());
			}

			return Activator.CreateInstance(objectType);
		}
	}
}