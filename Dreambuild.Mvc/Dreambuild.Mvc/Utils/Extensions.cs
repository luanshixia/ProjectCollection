using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Dreambuild.Mvc
{
    using System.Text.RegularExpressions;

    internal static class Extensions
    {
        public static string ToCamel(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = char.ToLowerInvariant(value[0]) + value.Substring(1);
            }

            return value;
        }

        public static string ToCamelRemoveHyphen(this string value) // yangwang@20141205
        {
            var regex = new Regex(@"\-[a-z]");
            value = regex.Replace(value, match => match.Value.Replace("-", "").ToUpper());
            return value.ToCamel();
        }

        public static string ToHyphenedName(this string value) // yangwang@20150529
        {
            var regex = new Regex("[A-Z]");
            return regex.Replace(value, match => "-" + match.Value.ToLower());
        }

        private static Regex _re = new Regex(@"^\p{Lu}|,\s*\p{Lu}");
        public static string ToCamel(this Enum value)
        {
            return _re.Replace(value.ToString(), match => match.Value.ToLower());
        }

        public static Double? ToNullableDouble(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return Double.Parse(value);
        }

        public static DateTime? ToNullableDateTime(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return DateTime.Parse(value);
        }

        public static string SafelyToString(this object source)
        {
            return source == null
                ? String.Empty
                : source.ToString();
        }

        public static T ToEnum<T>(this string source)
        {
            return String.IsNullOrEmpty(source)
                ? (T)Enum.ToObject(typeof(T), 0)
                : (T)Enum.Parse(typeof(T), source, true);
        }

        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static IDictionary<string, object> ToDictionary(this object @object)
        {
            var dictionary = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
            if (@object != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(@object))
                {
                    dictionary.Add(property.Name.Replace("_", "-"), property.GetValue(@object));
                }
            }
            return dictionary;
        }

        public static void ForEach<T>(this IEnumerable<T> instance, Action<T> action)
        {
            foreach (T item in instance)
            {
                action(item);
            }
        }
        public static double TryParseToDouble(this string source, double defaultValue = 0)
        {
            double result = defaultValue;
            double.TryParse(source, out result);
            return result;
        }

        public static int TryParseToInt32(this string source, int defaultValue = 0)
        {
            int result = defaultValue;
            int.TryParse(source, out result);
            return result;
        }

        public static string TryToString(this object source)
        {
            return Convert.ToString(source);
        }

        public static T ParseToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static bool CanParseToNumber(this string source)
        {
            double result = 0;
            return double.TryParse(source, out result);
        }

        public static List<T> WrapInList<T>(this T obj)
        {
            return new List<T> { obj };
        }

        public static T[] WrapInArray<T>(this T obj)
        {
            return new[] { obj };
        }
    }
}
