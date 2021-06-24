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
    public sealed class IntervalTree<T> where T : IComparable
    {
        private readonly IntervalTreeNode<T>? _root;

        public IntervalTree(IEnumerable<IInterval<T>> intervals, IntervalTreeMedianComputation<T> medianComputation)
        {
            var intervalsByStart = intervals.OrderBy(i => i.Start).ThenBy(i => i.End).ToList();
            var intervalsByEnd = intervalsByStart.OrderBy(i => i.End).ThenBy(i => i.Start).ToList();

            if (intervalsByStart.Count > 0)
                _root = new IntervalTreeNode<T>(intervalsByStart, intervalsByEnd, medianComputation);
        }

        public IList<IInterval<T>> GetBetween(T start, T end)
        {
            if (_root == null)
                return new List<IInterval<T>>();

            var list = new List<IInterval<T>>();
            _root.GetBetween(start, end, list);
            return list;
        }

        public IList<IInterval<T>> GetOverlapping(T start, T end)
        {
            if (_root == null)
                return new List<IInterval<T>>();

            var list = new List<IInterval<T>>();
            _root.GetOverlapping(start, end, list);
            return list;
        }

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
