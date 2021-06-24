using System;

namespace programmersdigest.Util.Collections
{
    /// <summary>
    /// Delegate for median computation functions used by the <see cref="IntervalTree{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input and output values.</typeparam>
    /// <param name="min">The lower bound of the interval to compute a median for.</param>
    /// <param name="max">The upper bound of the interval to compute a median for.</param>
    /// <returns>A median value between <paramref name="min" /> and <paramref name="max" />.</returns>
    public delegate T IntervalTreeMedianComputation<T>(T min, T max);

    /// <summary>
    /// Various common median computation functions for use in the <see cref="IntervalTree{T}"/>.
    /// </summary>
    public static class IntervalTreeMedianComputations
    {
        /// <summary>
        /// Computes the <see cref="short"/> median between <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="min">The lower bound of the interval to compute a median for.</param>
        /// <param name="max">The upper bound of the interval to compute a median for.</param>
        /// <returns>A median value between <paramref name="min" /> and <paramref name="max" />.</returns>
        public static short ShortMedian(short min, short max) => (short)((min + max) / 2);

        /// <summary>
        /// Computes the <see cref="int"/> median between <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="min">The lower bound of the interval to compute a median for.</param>
        /// <param name="max">The upper bound of the interval to compute a median for.</param>
        /// <returns>A median value between <paramref name="min" /> and <paramref name="max" />.</returns>
        public static int IntMedian(int min, int max) => (min + max) / 2;

        /// <summary>
        /// Computes the <see cref="long"/> median between <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="min">The lower bound of the interval to compute a median for.</param>
        /// <param name="max">The upper bound of the interval to compute a median for.</param>
        /// <returns>A median value between <paramref name="min" /> and <paramref name="max" />.</returns>
        public static long LongMedian(long min, long max) => (min + max) / 2L;

        /// <summary>
        /// Computes the <see cref="float"/> median between <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="min">The lower bound of the interval to compute a median for.</param>
        /// <param name="max">The upper bound of the interval to compute a median for.</param>
        /// <returns>A median value between <paramref name="min" /> and <paramref name="max" />.</returns>
        public static float FloatMedian(float min, float max) => (min + max) / 2f;

        /// <summary>
        /// Computes the <see cref="double"/> median between <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="min">The lower bound of the interval to compute a median for.</param>
        /// <param name="max">The upper bound of the interval to compute a median for.</param>
        /// <returns>A median value between <paramref name="min" /> and <paramref name="max" />.</returns>
        public static double DoubleMedian(double min, double max) => (min + max) / 2d;

        /// <summary>
        /// Computes the <see cref="decimal"/> median between <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="min">The lower bound of the interval to compute a median for.</param>
        /// <param name="max">The upper bound of the interval to compute a median for.</param>
        /// <returns>A median value between <paramref name="min" /> and <paramref name="max" />.</returns>
        public static decimal DecimalMedian(decimal min, decimal max) => (min + max) / 2m;

        /// <summary>
        /// Computes the <see cref="DateTime"/> median between <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="min">The lower bound of the interval to compute a median for.</param>
        /// <param name="max">The upper bound of the interval to compute a median for.</param>
        /// <returns>A median value between <paramref name="min" /> and <paramref name="max" />.</returns>
        public static DateTime DateTimeMedian(DateTime min, DateTime max) => min + (max - min);
    }
}
