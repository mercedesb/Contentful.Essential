using Contentful.CodeFirst;
using Contentful.Core.Models;
using Contentful.Essential.Models;
using Contentful.Essential.Models.Configuration;
using log4net.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Contentful.Essential.Utility
{
    public static class EntryExtensions
    {
        public static Entry<dynamic> GetDynamicEntry<T>(this T model, string locale = "") where T : IContentType
        {
            if (string.IsNullOrWhiteSpace(locale))
                locale = "en-US"; // TODO: set to space's default locale

            Entry<dynamic> entryWrapper = new Entry<dynamic>();
            entryWrapper.SystemProperties = model.Sys;

            // need to get locales
            var fields = new ExpandoObject() as IDictionary<string, object>;

            PropertyInfo[] props = model.GetType().GetProperties();
            Dictionary<string, object> localeValues;
            foreach (var prop in props)
            {
                if (prop.GetSetMethod() == null || prop.GetCustomAttribute<IgnoreContentFieldAttribute>() != null)
                    continue;

                localeValues = new Dictionary<string, object>();
                var fieldAttribute = prop.GetCustomAttribute<ContentFieldAttribute>() ?? new ContentFieldAttribute();
                localeValues.Add(locale, prop.GetValue(model));
                fields.Add(fieldAttribute.Id ?? prop.Name, localeValues);
            }
            entryWrapper.Fields = fields;

            return entryWrapper;
        }

        public static string GetContentTypeId<T>(this T model) where T : IContentType
        {
            ContentTypeAttribute contentTypeDef = (ContentTypeAttribute)Attribute.GetCustomAttribute(model.GetType(), typeof(ContentTypeAttribute));

            if (model.Sys != null)
                return model.Sys.ContentType.SystemProperties.Id;
            else if (contentTypeDef != null)
                return contentTypeDef.Id ?? model.GetType().Name;

            return string.Empty;
        }

        public static T ToDeliveryEntry<T>(this Entry<dynamic> model, string locale) where T : IContentType
        {
            JObject returnedFields = model.Fields as JObject;
            if (returnedFields != null)
            {
                try
                {
                    JsonSerializerSettings entryDynamicSerializerSettings = new JsonSerializerSettings();
                    entryDynamicSerializerSettings.Converters.Add(new EntryDynamicTypeJsonConverter());
                    Dictionary<string, T> allLocales = returnedFields.ToObject<Dictionary<string, T>>(JsonSerializer.Create(entryDynamicSerializerSettings));
                    if (!allLocales.ContainsKey(locale))
                    {
                        SystemLog.Log(typeof(EntryExtensions), $"Entry for locale {locale} does not exist. Returning default instead", Level.Warn);
                        return allLocales.First().Value;
                    }
                    return allLocales[locale];
                }
                catch (Exception ex)
                {
                    SystemLog.Log(typeof(EntryExtensions), $"Unable to deserialize CMA response to type {typeof(T)}", Level.Error, ex);
                }
            }
            return default(T);
        }

        public static T ToDeliveryEntry<T>(this IManagementContentType model, string locale) where T : class, IContentType, new()
        {
            try
            {
                Type deliveryType = typeof(T);
                T result = new T();
                foreach (PropertyInfo mgmtProp in model.GetType().GetProperties())
                {
                    Type mgmtPropType = mgmtProp.PropertyType;

                    // if the type of the property is not Dictionary<string, someType>, skip
                    if (!mgmtPropType.IsGenericType
                        || !typeof(Dictionary<,>).IsAssignableFrom(mgmtPropType.GetGenericTypeDefinition())
                        || !typeof(string).IsAssignableFrom(mgmtPropType.GetGenericArguments()[0]))
                        continue;

                    PropertyInfo deliveryProp = deliveryType.GetProperty(mgmtProp.Name, BindingFlags.Instance | BindingFlags.IgnoreCase);
                    // if a property with a matching name doesn't exist on the delivery type, skip
                    if (deliveryProp == null)
                        continue;

                    if (deliveryProp.GetSetMethod() == null || deliveryProp.GetCustomAttribute<IgnoreContentFieldAttribute>() != null)
                        continue;

                    // if mgmt prop's second dictionary argument type is not assignable from the delivery prop's type, skip
                    if (!mgmtPropType.GetGenericArguments()[1].GetType().IsAssignableFrom(deliveryProp.GetType()))
                        continue;

                    IDictionary mgmtPropValue = (IDictionary)mgmtProp.GetValue(model);
                    try
                    {
                        deliveryProp.SetValue(result, mgmtPropValue[locale]);
                    }
                    catch (Exception ex)
                    {
                        SystemLog.Log(typeof(EntryExtensions), $"Unable to set property {deliveryProp.Name} on object of type {typeof(T)} from object of type {model.GetType()}", Level.Error, ex);

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                SystemLog.Log(typeof(EntryExtensions), $"Unable to convert object of type {model.GetType()} to {typeof(T)}", Level.Error, ex);
                return default(T);
            }
        }
    }
}