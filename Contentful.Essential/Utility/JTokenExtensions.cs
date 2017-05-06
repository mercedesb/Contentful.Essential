using Newtonsoft.Json.Linq;
using System;

namespace Contentful.Essential.Utility
{
    public static class JTokenExtensions
    {
        /// <summary>
        /// Checks whether a JToken is null or of null type.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>Whether the token is null or not.</returns>
        public static bool IsNull(this JToken token)
        {
            return token == null || token.Type == JTokenType.Null;
        }

        /// <summary>
        /// Returns an int value from a JToken.
        /// </summary>
        /// <param name="token">The token to retrieve a value from.</param>
        /// <returns>The int value.</returns>
        public static int ToInt(this JToken token)
        {
            if (token.IsNull())
            {
                return 0;
            }

            return int.Parse(token.ToString());
        }

        /// <summary>
        /// Returns a nullable int value from a JToken.
        /// </summary>
        /// <param name="token">The token to retrieve a value from.</param>
        /// <returns>The nullable int value.</returns>
        public static int? ToNullableInt(this JToken token)
        {
            if (token.IsNull())
            {
                return new int?();
            }

            return new int?(token.ToInt());
        }

        public static bool ToBool(this JToken token)
        {
            if (token.IsNull())
                return false;

            return bool.Parse(token.ToString());
        }

        public static bool? ToNullableBool(this JToken token)
        {
            if (token.IsNull())
                return new bool?();

            return new bool?(token.ToBool());
        }

        public static float ToFloat(this JToken token)
        {
            if (token.IsNull())
                return 0;

            return float.Parse(token.ToString());
        }

        public static float? ToNullableFloat(this JToken token)
        {
            if (token.IsNull())
                return new float?();

            return new float?(token.ToFloat());
        }

        public static DateTime ToDateTime(this JToken token)
        {
            if (token.IsNull())
                return DateTime.MinValue;

            return DateTime.Parse(token.ToString());
        }

        public static DateTime? ToNullableDateTime(this JToken token)
        {
            if (token.IsNull())
                return new DateTime?();

            return new DateTime?(token.ToDateTime());
        }

        public static Guid ToGuid(this JToken token)
        {
            if (token.IsNull())
                return Guid.Empty;

            return Guid.Parse(token.ToString());
        }

        public static Guid? ToNullableGuid(this JToken token)
        {
            if (token.IsNull())
                return new Guid?();

            return new Guid?(token.ToGuid());
        }

        public static TimeSpan ToTimeSpan(this JToken token)
        {
            if (token.IsNull())
                return TimeSpan.MinValue;

            return TimeSpan.Parse(token.ToString());
        }

        public static TimeSpan? ToNullableTimeSpan(this JToken token)
        {
            if (token.IsNull())
                return new TimeSpan?();

            return new TimeSpan?(token.ToTimeSpan());
        }
    }
}