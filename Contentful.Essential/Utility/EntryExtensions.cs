using Contentful.CodeFirst;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Essential.Models;
using Contentful.Essential.Models.Configuration;
using Microsoft.Extensions.Logging;
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
        public static Entry<dynamic> GetDynamicEntry<T>(this T model, string locale) where T : IContentType
        {
            Entry<dynamic> entryWrapper = new Entry<dynamic>();
            entryWrapper.SystemProperties = model.Sys;

            // need to get locales
            var fields = new ExpandoObject() as IDictionary<string, object>;

            IEnumerable<PropertyInfo> props = model.GetType().GetTypeInfo().DeclaredProperties;
            Dictionary<string, object> localeValues;
            foreach (var prop in props)
            {
                if (prop.SetMethod == null || prop.GetCustomAttribute<IgnoreContentFieldAttribute>() != null)
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

            if (model.Sys != null)
                return model.Sys.ContentType.SystemProperties.Id;
            else
                return GetContentTypeId(model.GetType());
        }

        public static string GetContentTypeId(this Type contentType)
        {
            ContentTypeAttribute contentTypeDef = contentType.GetTypeInfo().GetCustomAttribute<ContentTypeAttribute>();
            if (contentTypeDef != null)
                return contentTypeDef.Id ?? contentType.Name;

            return string.Empty;
        }

       // public static T ToDeliveryEntry<T>(this Entry<dynamic> model, string locale, ILogger logger = null) where T : IContentType
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
                        //if (logger != null)
                        //    logger.LogWarning($"Entry for locale {locale} does not exist. Returning default instead");

                        return allLocales.First().Value;
                    }
                    return allLocales[locale];
                }
                catch (Exception ex)
                {
                    //if (logger != null)
                    //    logger.LogError(LoggingEvents.CDA_CMA_TypeConversionError, ex, $"Unable to deserialize CMA response to type {typeof(T)}");
                }
            }
            return default(T);
        }

        //public static T ToDeliveryEntry<T, V>(this Entry<V> model, string locale, ILogger logger = null)
        public static T ToDeliveryEntry<T, V>(this Entry<V> model, string locale)
            where T : class, IContentType, new()
            where V : class, IManagementContentType
        {
            try
            {
                Type deliveryType = typeof(T);
                T result = new T();
                foreach (PropertyInfo mgmtProp in model.Fields.GetType().GetTypeInfo().DeclaredProperties)
                {
                    Type mgmtPropType = mgmtProp.PropertyType;

                    // if the type of the property is not Dictionary<string, someType>, skip
                    if (!mgmtPropType.GetTypeInfo().IsGenericType
                        || !typeof(Dictionary<,>).GetTypeInfo().IsAssignableFrom(mgmtPropType.GetGenericTypeDefinition().GetTypeInfo())
                        || !typeof(string).GetTypeInfo().IsAssignableFrom(mgmtPropType.GetGenericArguments()[0].GetTypeInfo()))
                        continue;

                    //TODO: handle prop name casing
                    PropertyInfo deliveryProp = deliveryType.GetTypeInfo().GetDeclaredProperty(mgmtProp.Name);
                    // if a property with a matching name doesn't exist on the delivery type, skip
                    if (deliveryProp == null)
                        continue;

                    if (deliveryProp.SetMethod == null || deliveryProp.GetCustomAttribute<IgnoreContentFieldAttribute>() != null)
                        continue;

                    // if mgmt prop's second dictionary argument type is not assignable from the delivery prop's type, skip
                    if (!mgmtPropType.GetGenericArguments()[1].GetTypeInfo().IsAssignableFrom(deliveryProp.PropertyType.GetTypeInfo()))
                        continue;

                    IDictionary mgmtPropValue = (IDictionary)mgmtProp.GetValue(model.Fields);
                    try
                    {
                        deliveryProp.SetValue(result, mgmtPropValue[locale]);
                    }
                    catch (Exception ex)
                    {
                        //if (logger != null)
                        //    logger.LogError(LoggingEvents.CDA_CMA_TypeConversionError, ex, $"Unable to set property {deliveryProp.Name} on object of type {typeof(T)} from object of type {model.Fields.GetType()}");
                    }
                }
                result.Sys = model.SystemProperties;
                return result;
            }
            catch (Exception ex)
            {
                //if (logger != null)
                //    logger.LogError(LoggingEvents.CDA_CMA_TypeConversionError, ex, $"Unable to convert object of type {model.Fields.GetType()} to {typeof(T)}");
                return default(T);
            }
        }

        //public static Entry<T> ToManagementEntry<T, V>(this V model, string locale, ILogger logger = null)
        public static Entry<T> ToManagementEntry<T, V>(this V model, string locale)
            where T : class, IManagementContentType, new()
            where V : class, IContentType
        {
            try
            {
                Type mgmtType = typeof(T);
                T result = new T();
                foreach (PropertyInfo deliveryProp in model.GetType().GetTypeInfo().DeclaredProperties)
                {
                    // TODO: handle prop name casing
                    PropertyInfo mgmtProp = mgmtType.GetTypeInfo().GetDeclaredProperty(deliveryProp.Name);
                    // if a property with a matching name doesn't exist on the mgmt type, skip
                    if (mgmtProp == null)
                        continue;

                    if (mgmtProp.SetMethod == null)
                        continue;

                    Type mgmtPropType = mgmtProp.PropertyType;
                    // if the type of the property is not Dictionary<string, someType>, skip
                    if (!mgmtPropType.GetTypeInfo().IsGenericType
                        || !typeof(Dictionary<,>).GetTypeInfo().IsAssignableFrom(mgmtPropType.GetGenericTypeDefinition().GetTypeInfo())
                        || !typeof(string).GetTypeInfo().IsAssignableFrom(mgmtPropType.GetGenericArguments()[0].GetTypeInfo()))
                        continue;

                    // if mgmt prop's second dictionary argument type is not assignable from the delivery prop's type, skip
                    if (!mgmtPropType.GetGenericArguments()[1].GetTypeInfo().IsAssignableFrom(deliveryProp.PropertyType.GetTypeInfo()))
                        continue;

                    var dlvyPropValue = deliveryProp.GetValue(model);
                    try
                    {
                        var dictionaryType = typeof(Dictionary<,>).MakeGenericType(typeof(string), deliveryProp.PropertyType);
                        var mgmtPropValue = (IDictionary)Activator.CreateInstance(dictionaryType);
                        mgmtPropValue[locale] = dlvyPropValue;
                        mgmtProp.SetValue(result, mgmtPropValue);
                    }
                    catch (Exception ex)
                    {
                        //if (logger != null)
                        //    logger.LogError(LoggingEvents.CDA_CMA_TypeConversionError, ex, $"Unable to set property {mgmtProp.Name} on object of type {typeof(T)} from object of type {model.GetType()}");
                    }
                }
                Entry<T> entry = new Entry<T>();
                entry.SystemProperties = model.Sys;
                entry.Fields = result;
                return entry;
            }
            catch (Exception ex)
            {
                //if (logger != null)
                //    logger.LogError(LoggingEvents.CDA_CMA_TypeConversionError, ex, $"Unable to convert object of type {model.GetType()} to {typeof(T)}");

                return default(Entry<T>);
            }
        }

        public static void SetEntry<T>(this Dictionary<string, T> field, string entryId, string locale)
            where T : class, IContentType, new()
        {
            field[locale] = GetReferenceEntry<T>(entryId);
        }

        public static void AddEntryToArray<T>(this Dictionary<string, List<T>> field, string entryId, string locale)
            where T : class, IContentType, new()
        {
            if (field == null)
                field = new Dictionary<string, List<T>>();

            if (!field.ContainsKey(locale))
                field[locale] = new List<T>();

            List<T> entries = field[locale];

            if (entries == null)
                entries = new List<T>();

            entries.Add(GetReferenceEntry<T>(entryId));
        }

        public static T GetReferenceEntry<T>(this string entryId)
            where T : class, IContentType, new()
        {
            return new T()
            {
                Sys = new SystemProperties
                {
                    Id = entryId,
                    Type = SystemFieldTypes.Link,
                    LinkType = SystemLinkTypes.Entry,
                }
            };
        }
    }
}