using System;

namespace programmersdigest.Util.Collections
{
    /// <summary>
    /// Defines an interval which can be used in the <see cref="IntervalCollection{T}"/> and <see cref="IntervalTree{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInterval<T> where T : IComparable
    {
        /// <summary>
        /// The lower bound (including) of the interval.
        /// </summary>
        T Start { get; }

        /// <summary>
        /// The upper bound (including) of the interval.
        /// </summary>
        T End { get; }
    }
}
