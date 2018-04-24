using System;

namespace Dreambuild.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="String"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Tries parsing to <see cref="Double"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        public static double TryParseToDouble(this string s, double defaultValue = 0)
        {
            double result = defaultValue;
            double.TryParse(s, out result);
            return result;
        }

        /// <summary>
        /// Tries parsing to <see cref="Int32"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        public static int TryParseToInt32(this string s, int defaultValue = 0)
        {
            int result = defaultValue;
            int.TryParse(s, out result);
            return result;
        }

        /// <summary>
        /// Parses to enum.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="value">The string value.</param>
        /// <returns>The result.</returns>
        public static T ParseToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Determines if can parse to number.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>A boolean result.</returns>
        public static bool CanParseToNumber(this string s)
        {
            return double.TryParse(s, out double result);
        }
    }
}
