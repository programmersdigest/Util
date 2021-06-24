using System;
using System.Collections.Generic;
using System.Linq;

namespace programmersdigest.Util.Collections
{
    /// <summary>
    /// A generic data structure to store and lookup sequenced interval data (e.g. number ranges, time ranges, ...).
    /// Allows for fast querying of intervals contained within, overlapping or enclosing a given interval.
    /// For some data, <see cref="IntervalCollection{T}"/> may yield even better performance (ca. factor 10 faster).
    /// </summary>
    /// <typeparam name="T">The type of the start and end points of the contained <see cref="IInterval{T}"/>s.</typeparam>
    public sealed class IntervalTree<T> where T : IComparable
    {
        private readonly IntervalTreeNode<T>? _root;

        /// <summary>
        /// Creates a new <see cref="IntervalTree{T}"/> from the unordered collection of intervals.
        /// </summary>
        /// <param name="intervals">An unordered collection of intervals.</param>
        /// <param name="medianComputation">
        /// A function to compute the median between a given start and end point. Some common median computations are
        /// defined in <see cref="IntervalTreeMedianComputations"/>.
        /// </param>
        /// <returns>An <see cref="IntervalTree{T}"/> containing all items from <paramref name="intervals"/></returns>
        public IntervalTree(IEnumerable<IInterval<T>> intervals, IntervalTreeMedianComputation<T> medianComputation)
        {
            var intervalsByStart = intervals.OrderBy(i => i.Start).ThenBy(i => i.End).ToList();
            var intervalsByEnd = intervalsByStart.OrderBy(i => i.End).ThenBy(i => i.Start).ToList();

            if (intervalsByStart.Count > 0)
                _root = new IntervalTreeNode<T>(intervalsByStart, intervalsByEnd, medianComputation);
        }

        /// <summary>
        /// Retrieves all intervals which lay within the search interval.
        /// </summary>
        /// <example>
        /// Given the following intervals [0]-[4] and the search interval [x]:
        /// [0] |-------|
        /// [1]     |-----|
        /// [2]        |----|
        /// [3]          |----|
        /// [4]           |-------|
        /// [x]     |---------|
        /// [1], [2], [3] would be included in the result set. [0], [4] would not be included.
        /// </example>
        /// <param name="start">The lower boundary of the search interval.</param>
        /// <param name="end">The upper bound of the search interval.</param>
        /// <returns>A list containing (in no particular order) all intervals which lay in the search interval.</returns>
        public IList<IInterval<T>> GetBetween(T start, T end)
        {
            if (_root == null)
                return new List<IInterval<T>>();

            var list = new List<IInterval<T>>();
            _root.GetBetween(start, end, list);
            return list;
        }

        /// <summary>
        /// Retrieves all intervals which overlap with the search interval (including just touching the start or end points).
        /// </summary>
        /// <example>
        /// Given the following intervals [0]-[6] and the search interval [x]:
        /// [0] |--|
        /// [1]  |--|
        /// [2]   |----|
        /// [3]       |----|
        /// [4]           |-------|
        /// [5]               |----|
        /// [6]                |-----|
        /// [x]     |---------|
        /// [1], [2], [3], [4], [5] would be included in the result set. [0], [6] would not be included.
        /// </example>
        /// <param name="start">The lower boundary of the search interval.</param>
        /// <param name="end">The upper bound of the search interval.</param>
        /// <returns>A list containing (in no particular order) all intervals which overlap with the search interval.</returns>
        public IList<IInterval<T>> GetOverlapping(T start, T end)
        {
            if (_root == null)
                return new List<IInterval<T>>();

            var list = new List<IInterval<T>>();
            _root.GetOverlapping(start, end, list);
            return list;
        }

        /// <summary>
        /// Retrieves all intervals which enclose the search interval.
        /// </summary>
        /// <example>
        /// Given the following intervals [0]-[4] and the search interval [x]:
        /// [0] |--------|
        /// [1]  |----------------|
        /// [2]     |---------|
        /// [3]       |----|
        /// [4]           |-------|
        /// [x]     |---------|
        /// [1], [2] would be included in the result set. [0], [3], [4] would not be included.
        /// </example>
        /// <param name="start">The lower boundary of the search interval.</param>
        /// <param name="end">The upper bound of the search interval.</param>
        /// <returns>A list containing (in no particular order) all intervals which overlap with the search interval.</returns>
        public IList<IInterval<T>> GetEnclosing(T start, T end)
        {
            if (_root == null)
                return new List<IInterval<T>>();

            var list = new List<IInterval<T>>();
            _root.GetEnclosing(start, end, list);
            return list;
        }
    }
}
