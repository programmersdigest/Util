using System;
using System.Collections.Generic;
using System.Linq;

namespace programmersdigest.Util.Collections
{
    /// <summary>
    /// A node in the <see cref="IntervalTree{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the start and end points of the contained <see cref="IInterval{T}"/>s.</typeparam>
    internal sealed class IntervalTreeNode<T> where T : IComparable
    {
        /// <summary>
        /// The median between the outer interval bounds <see cref="IntervalMax"/> and <see cref="IntervalMax"/>.
        /// </summary>
        public T Median { get; }

        /// <summary>
        /// The lower bound (including) of all contained intervals (which includes all items in <see cref="OverlappingByStart"/>/<see cref="OverlappingByEnd"/>
        /// as well as in <see cref="Left"/>).
        /// Guaranteed to be <= <see cref="OverlappingMin"/> and <= <see cref="Median"/>.
        /// </summary>
        public T IntervalMin { get; }

        /// <summary>
        /// The upper bound (including) of all contained intervals (which includes all items in <see cref="OverlappingByStart"/>/<see cref="OverlappingByEnd"/>
        /// as well as in <see cref="Right"/>).
        /// Guaranteed to be >= <see cref="OverlappingMax"/> and >= <see cref="Median"/>.
        /// </summary>
        public T IntervalMax { get; }

        /// <summary>
        /// The lower bound (including) of all intervals in <see cref="OverlappingByStart"/>/<see cref="OverlappingByEnd"/>.
        /// Guaranteed to be >= <see cref="IntervalMin"/> and <= <see cref="Median"/>.
        /// </summary>
        public T OverlappingMin => OverlappingByStart is null ? Median : OverlappingByStart.First().Start;

        /// <summary>
        /// The upper bound (including) of all intervals in <see cref="OverlappingByStart"/>/<see cref="OverlappingByEnd"/>.
        /// Guaranteed to be <= <see cref="OverlappingMax"/> and >= <see cref="Median"/>.
        /// </summary>
        public T OverlappingMax => OverlappingByEnd is null ? Median : OverlappingByEnd.Last().End;

        /// <summary>
        /// Contains all intervals which touch or overlap with <see cref="Median"/> sorted by <see cref="IInterval{T}.Start"/> ASC,
        /// <see cref="IInterval{T}.End"/> ASC.
        /// </summary>
        public List<IInterval<T>>? OverlappingByStart { get; }

        /// <summary>
        /// Contains all intervals which touch or overlap with <see cref="Median"/> sorted by <see cref="IInterval{T}.End"/> ASC,
        /// <see cref="IInterval{T}.Start"/> ASC.
        /// </summary>
        public List<IInterval<T>>? OverlappingByEnd { get; }

        /// <summary>
        /// A sub node containing items which lay left of <see cref="Median"/>.
        /// </summary>
        public IntervalTreeNode<T>? Left { get; }

        /// <summary>
        /// A sub node containing items which lay right of <see cref="Median"/>.
        /// </summary>
        public IntervalTreeNode<T>? Right { get; }

        /// <summary>
        /// Creates a new <see cref="IntervalTreeNode{T}"/> from the given sorted collections of intervals.
        /// <paramref name="intervalsByStart"/> and <paramref name="intervalsByEnd"/> must contain the same items!
        /// </summary>
        /// <param name="intervalsByStart">
        /// A collection of items to contain in this node sorted by <see cref="IInterval{T}.Start"/> ASC,
        /// <see cref="IInterval{T}.End"/> ASC.
        /// </param>
        /// <param name="intervalsByEnd">
        /// A collection of items to contain in this node sorted by <see cref="IInterval{T}.End"/> ASC,
        /// <see cref="IInterval{T}.Start"/> ASC.
        /// </param>
        /// <param name="medianComputation">
        /// A function to compute the median between a given start and end point. Some common median computations are
        /// defined in <see cref="IntervalTreeMedianComputations"/>.
        /// </param>
        public IntervalTreeNode(IList<IInterval<T>> intervalsByStart, IList<IInterval<T>> intervalsByEnd, IntervalTreeMedianComputation<T> medianComputation)
        {
            IntervalMin = intervalsByStart.First().Start;
            IntervalMax = intervalsByEnd.Last().End;
            Median = medianComputation(IntervalMin, IntervalMax);

            var leftByStart = new List<IInterval<T>>();
            var leftByEnd = new List<IInterval<T>>();
            var rightByStart = new List<IInterval<T>>();
            var rightByEnd = new List<IInterval<T>>();
            var overlappingByStart = new List<IInterval<T>>();
            var overlappingByEnd = new List<IInterval<T>>();

            foreach (var interval in intervalsByStart)
            {
                if (interval.End.CompareTo(Median) < 0)
                    leftByStart.Add(interval);
                else if (interval.Start.CompareTo(Median) > 0)
                    rightByStart.Add(interval);
                else
                    overlappingByStart.Add(interval);
            }

            foreach (var interval in intervalsByEnd)
            {
                if (interval.End.CompareTo(Median) < 0)
                    leftByEnd.Add(interval);
                else if (interval.Start.CompareTo(Median) > 0)
                    rightByEnd.Add(interval);
                else
                    overlappingByEnd.Add(interval);
            }

            if (overlappingByStart.Count > 0)
            {
                OverlappingByStart = overlappingByStart;
                OverlappingByEnd = overlappingByEnd;
            }

            if (leftByStart.Count > 0)
                Left = new IntervalTreeNode<T>(leftByStart, leftByEnd, medianComputation);
            if (rightByStart.Count > 0)
                Right = new IntervalTreeNode<T>(rightByStart, rightByEnd, medianComputation);
        }

        /// <summary>
        /// Retrieves all intervals which lay within the search interval and adds them to the given list.
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
        /// <param name="list">A list to which resulting intervals can be added. Items are added in no particular order!</param>
        public void GetBetween(T start, T end, List<IInterval<T>> list)
        {
            if (Left != null && Left.IntervalMax.CompareTo(start) >= 0)
                Left.GetBetween(start, end, list);
            if (Right != null && Right.IntervalMin.CompareTo(end) <= 0)
                Right.GetBetween(start, end, list);

            if (OverlappingByStart is null || OverlappingByEnd is null || Median.CompareTo(start) < 0 || Median.CompareTo(end) > 0)
                return;

            if (OverlappingMin.CompareTo(start) >= 0)
            {
                var index = 0;
                while (index < OverlappingByEnd.Count && OverlappingByEnd[index].End.CompareTo(end) <= 0)
                    index++;
                list.AddRange(OverlappingByEnd.GetRange(0, index));
            }
            else if (OverlappingMax.CompareTo(end) <= 0)
            {
                var index = OverlappingByStart.Count - 1;
                while (index >= 0 && OverlappingByStart[index].Start.CompareTo(start) >= 0)
                    index--;
                index++;
                list.AddRange(OverlappingByStart.GetRange(index, OverlappingByStart.Count - index));
            }
            else
                list.AddRange(OverlappingByStart.Where(i => i.Start.CompareTo(start) >= 0 && i.End.CompareTo(end) <= 0));
        }

        /// <summary>
        /// Retrieves all intervals which overlap with the search interval (including just touching the start or end points)
        /// and adds them to the given list.
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
        /// <param name="list">A list to which resulting intervals can be added. Items are added in no particular order!</param>
        public void GetOverlapping(T start, T end, List<IInterval<T>> list)
        {
            if (Left != null && Left.IntervalMax.CompareTo(start) >= 0)
                Left.GetOverlapping(start, end, list);
            if (Right != null && Right.IntervalMin.CompareTo(end) <= 0)
                Right.GetOverlapping(start, end, list);

            if (OverlappingByStart is null || OverlappingByEnd is null || OverlappingMax.CompareTo(start) < 0 || OverlappingMin.CompareTo(end) > 0)
                return;

            if (OverlappingMin.CompareTo(start) >= 0)
            {
                var index = 0;
                while (index < OverlappingByStart.Count && OverlappingByStart[index].Start.CompareTo(end) <= 0)
                    index++;
                list.AddRange(OverlappingByStart.GetRange(0, index));
            }
            else if (OverlappingMax.CompareTo(end) <= 0)
            {
                var index = OverlappingByEnd.Count - 1;
                while (index >= 0 && OverlappingByEnd[index].End.CompareTo(start) >= 0)
                    index--;
                index++;
                list.AddRange(OverlappingByEnd.GetRange(index, OverlappingByEnd.Count - index));
            }
            else
                list.AddRange(OverlappingByStart.Where(i => i.End.CompareTo(start) >= 0 && i.Start.CompareTo(end) <= 0));
        }

        /// <summary>
        /// Retrieves all intervals which enclose the search interval and adds them to the given list.
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
        /// <param name="list">A list to which resulting intervals can be added. Items are added in no particular order!</param>
        public void GetEnclosing(T start, T end, List<IInterval<T>> list)
        {
            if (Left != null && Left.IntervalMax.CompareTo(start) >= 0 && Left.IntervalMax.CompareTo(end) >= 0)
                Left.GetEnclosing(start, end, list);
            if (Right != null && Right.IntervalMin.CompareTo(end) <= 0 && Right.IntervalMin.CompareTo(start) <= 0)
                Right.GetEnclosing(start, end, list);

            if (OverlappingByStart is null || OverlappingMin.CompareTo(start) > 0 || OverlappingMax.CompareTo(end) < 0)
                return;

            list.AddRange(OverlappingByStart.Where(i => i.Start.CompareTo(start) <= 0 && i.End.CompareTo(end) >= 0));
        }
    }
}
