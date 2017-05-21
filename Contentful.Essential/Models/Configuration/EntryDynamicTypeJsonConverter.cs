using Contentful.Essential.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Contentful.Essential.Models.Configuration
{
    public class EntryDynamicTypeJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType
                && typeof(Dictionary<,>).IsAssignableFrom(objectType.GetGenericTypeDefinition())
                && typeof(string).IsAssignableFrom(objectType.GetGenericArguments()[0])
                && typeof(IContentType).IsAssignableFrom(objectType.GetGenericArguments()[1]);
        }

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
            Type keyType = objectType.GetGenericArguments()[0];
            Type valueType = objectType.GetGenericArguments()[1];
            var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            var result = (IDictionary)Activator.CreateInstance(dictionaryType);

            var jsonObject = JObject.Load(reader);
            JToken jToken;

            foreach (PropertyInfo prop in valueType.GetProperties())
            {
                if (prop.GetSetMethod() == null)
                    continue;

                jsonObject.TryGetValue(prop.Name, StringComparison.InvariantCultureIgnoreCase, out jToken);

                if (jToken != null)
                {
                    foreach (JProperty localeValue in jToken.Children<JProperty>())
                    {
                        object entry;
                        if (result.Contains(localeValue.Name))
                            entry = result[localeValue.Name];
                        else
                            entry = Activator.CreateInstance(valueType);


                        if (localeValue.Value == null || localeValue.Value.Type == JTokenType.Null || localeValue.Value.Type == JTokenType.Undefined)
                        {
                            if (!prop.PropertyType.IsValueType || (Nullable.GetUnderlyingType(prop.PropertyType) != null))
                                prop.SetValue(entry, null);
                        }
                        else
                        {
                            SetPropertyValue(localeValue, prop, entry);
                        }

                        result[localeValue.Name] = entry;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        protected virtual void SetPropertyValue(JProperty localeValue, PropertyInfo prop, object result)
        {
            if (localeValue.Value == null || localeValue.Value.Type == JTokenType.Null || localeValue.Value.Type == JTokenType.Undefined)
            {
                if (!prop.PropertyType.IsValueType || (Nullable.GetUnderlyingType(prop.PropertyType) != null))
                {
                    prop.SetValue(result, null);
                }
                return;
            }

            switch (localeValue.Value.Type)
            {
                case (JTokenType.String):
                    if (typeof(string).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, localeValue.Value.ToString());
                    }
                    break;
                case (JTokenType.Boolean):
                    if (typeof(bool).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, localeValue.Value.ToBool());
                    }
                    else if (typeof(bool?).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, localeValue.Value.ToNullableBool());
                    }
                    break;
                case (JTokenType.Date):
                    if (typeof(DateTime).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, localeValue.Value.ToDateTime());
                    }
                    else if (typeof(DateTime?).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, localeValue.Value.ToNullableDateTime());
                    }
                    break;
                case (JTokenType.Float):
                    if (typeof(float).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, localeValue.Value.ToFloat());
                    }
                    else if (typeof(float?).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, localeValue.Value.ToNullableFloat());
                    }
                    else if (typeof(double).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, (double)localeValue.Value.ToFloat());
                    }
                    else if (typeof(double?).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, (double?)localeValue.Value.ToNullableFloat());
                    }
                    else if (typeof(decimal).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, (decimal)localeValue.Value.ToFloat());
                    }
                    else if (typeof(decimal?).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, (decimal?)localeValue.Value.ToNullableFloat());
                    }
                    break;
                case (JTokenType.Guid):
                    if (typeof(Guid).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, localeValue.Value.ToGuid());
                    }
                    else if (typeof(Guid?).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, localeValue.Value.ToNullableGuid());
                    }
                    break;
                case (JTokenType.Integer):
                    if (typeof(int).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, localeValue.Value.ToInt());
                    }
                    else if (typeof(int?).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, localeValue.Value.ToNullableInt());
                    }
                    break;
                case (JTokenType.TimeSpan):
                    if (typeof(TimeSpan).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, localeValue.Value.ToTimeSpan());
                    }
                    else if (typeof(TimeSpan?).IsAssignableFrom(prop.PropertyType))
                    {
                        prop.SetValue(result, localeValue.Value.ToNullableTimeSpan());
                    }
                    break;
                case (JTokenType.Array):
                case (JTokenType.Object):
                    var obj = (localeValue.Value as JObject).ToObject(prop.PropertyType);
                    prop.SetValue(result, obj);
                    break;
                // do nothing, not sure what else to do...
                case (JTokenType.Bytes):
                case (JTokenType.Comment):
                case (JTokenType.Constructor):
                case (JTokenType.None):
                case (JTokenType.Property):
                case (JTokenType.Uri):
                case (JTokenType.Raw):
                    break;
            }
        }
    }
}