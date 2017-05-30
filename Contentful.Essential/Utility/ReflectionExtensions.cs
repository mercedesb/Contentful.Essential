using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Contentful.Essential.Utility
{
	public static class ReflectionExtensions
	{
        /// <summary>
        /// Extension method to replicate .Net Framework Type.GetGenericArguments() behavior in .NET Core
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition
                ? type.GetTypeInfo().GenericTypeParameters
                : type.GetTypeInfo().GenericTypeArguments;
        }
	}
}
