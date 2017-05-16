using Contentful.CodeFirst;
using Contentful.Core.Models;
using Contentful.Essential.Models;
using System.Collections.Generic;
using System.Dynamic;
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

            // need to get locales (waiting to hear from Contentful)
            var fields = new ExpandoObject() as IDictionary<string, object>;

            PropertyInfo[] props = model.GetType().GetProperties();
            Dictionary<string, object> localeValues;
            foreach (var prop in props)
            {
                if (prop.GetSetMethod() == null || prop.GetCustomAttribute<IgnoreContentFieldAttribute>() != null)
                    continue;

                localeValues = new Dictionary<string, object>();
                localeValues.Add(locale, prop.GetValue(model));
                fields.Add(prop.Name, localeValues);
            }
            entryWrapper.Fields = fields;

            return entryWrapper;
        }
    }
}