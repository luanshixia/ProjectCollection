namespace CosmosDBQueryCharge
{
    using System;
    using System.Threading;

    /// <summary>
    /// Random number generator.
    /// </summary>
    public static class RandomNumber
    {
        /// <summary>
        /// The thread local instance of random number generator.
        /// </summary>
        private static readonly ThreadLocal<Random> ThreadLocal = new ThreadLocal<Random>(
            () => new Random(Interlocked.Increment(ref seed)));

        /// <summary>
        /// Random number generator seed value.
        /// </summary>
        private static int seed = Environment.TickCount;

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        public static double NextDouble()
        {
            return RandomNumber.ThreadLocal.Value.NextDouble();
        }

        /// <summary>
        /// Produces 32-bit nonnegative random number.
        /// </summary>
        public static int Next()
        {
            return RandomNumber.ThreadLocal.Value.Next();
        }

        /// <summary>
        /// Produces 32-bit nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The max value.</param>
        public static int Next(int maxValue)
        {
            return RandomNumber.ThreadLocal.Value.Next(maxValue);
        }

        /// <summary>
        /// Produces 32-bit nonnegative random number within a specified range.
        /// </summary>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        public static int Next(int minValue, int maxValue)
        {
            return RandomNumber.ThreadLocal.Value.Next(minValue, maxValue);
        }

        /// <summary>
        /// Produces the random interval less than the specified maximum.
        /// </summary>
        /// <param name="maximum">The maximum value.</param>
        public static TimeSpan Next(TimeSpan maximum)
        {
            return RandomNumber.Next(TimeSpan.Zero, maximum);
        }

        /// <summary>
        /// Produces the random interval within a specified range.
        /// </summary>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maximum">The maximum value.</param>
        public static TimeSpan Next(TimeSpan minimum, TimeSpan maximum)
        {
            return TimeSpan.FromTicks(minimum.Ticks + (long)(RandomNumber.NextDouble() * (maximum.Ticks - minimum.Ticks)));
        }
    }
}
