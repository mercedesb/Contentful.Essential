using System;

namespace Contentful.Essential.Utility
{
    public static class StringExtensions
    {
        // Convert the string to camel case.
        public static string ToCamelCase(this string stringToConvert)
        {
            // If there are 0 or 1 characters, just return the string.
            if (stringToConvert == null || stringToConvert.Length < 2)
                return stringToConvert;

            // Split the string into words.
            string[] words = stringToConvert.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = words[0].ToLower();
            for (int i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);
            }

            return result;
        }
    }
}