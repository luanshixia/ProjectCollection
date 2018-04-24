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
        /// <returns>The result.</returns>
        public static List<T> WrapInList<T>(this T value)
        {
            return new List<T> { value };
        }

        /// <summary>
        /// Wraps value in an array.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The result.</returns>
        public static T[] WrapInArray<T>(this T value)
        {
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
    }
}
