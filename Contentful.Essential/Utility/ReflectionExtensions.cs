using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Contentful.Essential.Utility
{
	public static class ReflectionExtensions
	{
		/// <summary>
		/// Will return a list of <typeparamref name="TResult"/>
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="obj"></param>
		/// <param name="parser">A parser to parse the propertyinfo and return a <typeparamref name="TResult"/></param>
		/// <returns></returns>
		public static IEnumerable<TResult> GetProperties<TResult>(this object obj, Func<PropertyInfo, TResult> parser)
		{
			foreach (var prop in obj.GetType().GetCachedProperties())
			{
				yield return parser(prop);
			}
		}

		private static ConcurrentDictionary<Type, PropertyInfo[]> _propertiesDictionary = new ConcurrentDictionary<Type, PropertyInfo[]>();

		/// <summary>
		/// Will get a types properties from internal mem cache, if not found it will add it
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static PropertyInfo[] GetCachedProperties(this Type type)
		{
			if (_propertiesDictionary.ContainsKey(type))
				return _propertiesDictionary[type];

			var properties = type.GetProperties();
			_propertiesDictionary.TryAdd(type, properties);
			return properties;
		}
	}
}
