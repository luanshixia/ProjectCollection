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
        /// Tries parsing to <see cref="Int64"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The result.</returns>
        public static long TryParseToInt64(this string s, long defaultValue = 0)
        {
            long result = defaultValue;
            long.TryParse(s, out result);
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
        /// Parses to <see cref="Double"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>The result.</returns>
        public static double ParseToDouble(this string s)
        {
            return double.Parse(s);
        }

        /// <summary>
        /// Parses to <see cref="Int64"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>The result.</returns>
        public static long ParseToInt64(this string s)
        {
            return long.Parse(s);
        }

        /// <summary>
        /// Parses to <see cref="Int32"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>The result.</returns>
        public static int ParseToInt32(this string s)
        {
            return int.Parse(s);
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
            return s.CanParseToDouble() || s.CanParseToInt64();
        }

        /// <summary>
        /// Determines if can parse to <see cref="Double"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>A boolean result.</returns>
        public static bool CanParseToDouble(this string s)
        {
            return double.TryParse(s, out double result);
        }

        /// <summary>
        /// Determines if can parse to <see cref="Int64"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>A boolean result.</returns>
        public static bool CanParseToInt64(this string s)
        {
            return long.TryParse(s, out long result);
        }

        /// <summary>
        /// Determines if can parse to <see cref="Int32"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>A boolean result.</returns>
        public static bool CanParseToInt32(this string s)
        {
            return int.TryParse(s, out int result);
        }
    }
}
