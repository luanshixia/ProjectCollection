using Dreambuild.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dreambuild.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Object"/>.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Safe ToString().
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result.</returns>
        public static string TryToString(this object value)
        {
            return Convert.ToString(value);
        }

        /// <summary>
        /// Wraps value in a list.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="emptyIfNull">If true, return empty list for null value.</param>
        /// <returns>The result.</returns>
        public static List<T> WrapInList<T>(this T value, bool emptyIfNull = false)
        {
            if (emptyIfNull && value == null)
            {
                return EmptyList<T>.Instance;
            }

            return new List<T> { value };
        }

        /// <summary>
        /// Wraps value in an array.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="emptyIfNull">If true, return empty array for null value.</param>
        /// <returns>The result.</returns>
        public static T[] WrapInArray<T>(this T value, bool emptyIfNull = false)
        {
            if (emptyIfNull && value == null)
            {
                return Array.Empty<T>();
            }

            return new[] { value };
        }

        /// <summary>
        /// Gets property value from an object.
        /// </summary>
        /// <param name="object">The object.</param>
        /// <param name="path">The property path.</param>
        /// <returns>The property value.</returns>
        public static object GetPropertyValue(this object @object, string path)
        {
            var paths = path.Split('.');
            var host = @object;
            foreach (var prop in paths)
            {
                host = host.GetType().GetRuntimeProperty(prop).GetValue(host);
            }
            return host;
        }

        /// <summary>
        /// Adds element to a dictionary of List value.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the List element.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The element value.</param>
        public static void AddElement<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = new List<TValue>();
            }

            dictionary[key].Add(value);
        }

        /// <summary>
        /// Adds entry to a dictionary of Dictionary value.
        /// </summary>
        /// <typeparam name="TKey1">The type of the key 1.</typeparam>
        /// <typeparam name="TKey2">The type of the key 2.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key1">The key 1.</param>
        /// <param name="key2">The key 2.</param>
        /// <param name="value">The entry value.</param>
        public static void AddEntry<TKey1, TKey2, TValue>(this IDictionary<TKey1, Dictionary<TKey2, TValue>> dictionary, TKey1 key1, TKey2 key2, TValue value)
        {
            if (!dictionary.ContainsKey(key1))
            {
                dictionary[key1] = new Dictionary<TKey2, TValue>();
            }

            dictionary[key1][key2] = value;
        }
    }
}
