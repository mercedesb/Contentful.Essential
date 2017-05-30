using System;
using System.Globalization;

namespace Contentful.Essential.Utility
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert a string to basic Title Case
        /// </summary>
        /// <param name="stringToConvert"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string stringToConvert)
        {
            string[] tokens = stringToConvert.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                
                tokens[i] = token.Substring(0, 1).ToUpper() + token.Substring(1).ToLower();
            }

            return string.Join(" ", tokens);
        }

        /// <summary>
        /// Capitalize first letter of every word
        /// </summary>
        /// <param name="stringToConvert"></param>
        /// <returns></returns>
        public static string CapitalizeFirstLetterInWords(this string stringToConvert)
        {
            string[] tokens = stringToConvert.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];

                // input: FieldName, result: FieldName
                tokens[i] = token.Substring(0, 1).ToUpper() + token.Substring(1);
            }

            return string.Join(" ", tokens);
        }

        /// <summary>
        /// Convert a string to PascalCase
        /// </summary>
        /// <param name="stringToConvert"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string stringToConvert)
        {
            // if more than one word, convert to title case
            if (stringToConvert.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                stringToConvert = stringToConvert.ToTitleCase();
            // if one word, just capitalize string (to maintain casing of property names)
            else
                stringToConvert = stringToConvert.CapitalizeFirstLetterInWords();

            string[] tokens = stringToConvert.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string result = String.Join(string.Empty, tokens);
            return result;
        }

        /// <summary>
        /// Convert a string to camelCase
        /// </summary>
        /// <param name="stringToConvert"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string stringToConvert)
        {
            stringToConvert = stringToConvert.ToPascalCase();
            return stringToConvert.Substring(0, 1).ToLower() +
                stringToConvert.Substring(1);
        }
    }
}