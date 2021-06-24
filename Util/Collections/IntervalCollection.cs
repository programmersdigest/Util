using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace programmersdigest.Util.Collections
{
    /// <summary>
    /// A generic data structure to store and lookup sequenced interval data (e.g. number ranges, time ranges, ...).
    /// Allows for very fast querying of intervals contained within, overlapping or enclosing a given interval.
    /// 
    /// NOT SUPPORTED: Intervals which fully enclose other intervals! Every interval n MUST have
    ///     Start >= interval[n-1] && End >= interval[n-1]
    /// 
    /// [0] |------|
    /// [1] |--------|      <- OK
    /// [2]   |------|      <- OK
    /// [3]    |----|       <- NOT OK
    /// 
    /// If you cannot rule out these constellations, you can use the <see cref="IntervalTree{T}"/> instead.
    /// </summary>
    /// <typeparam name="T">The type of the start and end points of the contained <see cref="IInterval{T}"/>s.</typeparam>
    public sealed class IntervalCollection<T> : ICollection<IInterval<T>>, IEnumerable<IInterval<T>> where T : IComparable
    {
        private delegate int Comparator(IInterval<T> interval, T start, T end);

        /// <summary>
        /// Creates a new <see cref="IntervalCollection{T}"/> from the ordered collection of intervals.
        /// </summary>
        /// <param name="intervals">A collection of intervals ordered by Start ASC, End ASC.</param>
        /// <returns>A collection containing all items from <paramref name="intervals"/></returns>
        public static IntervalCollection<T> FromOrdered(IEnumerable<IInterval<T>> intervals)
        {
            return new IntervalCollection<T>(intervals.ToList());
        }

        /// <summary>
        /// Creates a new <see cref="IntervalCollection{T}"/> from the unordered collection of intervals.
        /// </summary>
        /// <param name="intervals">An unordered  collection of intervals.</param>
        /// <returns>A collection containing all items from <paramref name="intervals"/></returns>
        public static IntervalCollection<T> FromUnordered(IEnumerable<IInterval<T>> intervals)
        {
            intervals = intervals.OrderBy(i => i.Start).ThenBy(i => i.End);
            return new IntervalCollection<T>(intervals.ToList());
        }

        private readonly List<IInterval<T>> _intervals;

        /// <summary>
        /// Gets the number of intervals contained in the collection.
        /// </summary>
        public int Count => _intervals.Count;

        /// <summary>
        /// Always <c>false</c>.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Retrieves an interval by index.
        /// </summary>
        /// <param name="index">The index of the item to retrieve.</param>
        /// <returns>The interval at <paramref name="index"/>.</returns>
        public IInterval<T> this[int index] => _intervals[index];

        /// <summary>
        /// Creates an empty <see cref="IntervalCollection{T}"/>.
        /// For creation with an ordered or unordered set of intervals, use <see cref="FromOrdered(IEnumerable{IInterval{T}})"/>
        /// and <see cref="FromUnordered(IEnumerable{IInterval{T}})"/>.
        /// </summary>
        public IntervalCollection() : this(new List<IInterval<T>>())
        {
        }

        private IntervalCollection(List<IInterval<T>> intervals)
        {
            _intervals = intervals;
        }

        /// <summary>
        /// Adds an interval to the collection.
        /// </summary>
        /// <param name="interval">The interval to add.</param>
        public void Add(IInterval<T> interval)
        {
            var index = BinarySearch(_intervals, SearchInsertIndexComparator, interval.Start, interval.End);
            if (index < 0)
                index = ~index;

            _intervals.Insert(index, interval);
        }

        /// <summary>
        /// Adds a collection of intervals to the collection.
        /// </summary>
        /// <param name="intervals">The collection of intervals to add.</param>
        public void AddRange(IEnumerable<IInterval<T>> intervals)
        {
            foreach (var interval in intervals)
                Add(interval);
        }

        /// <summary>
        /// Removes all intervals from the collection.
        /// </summary>
        public void Clear()
        {
            _intervals.Clear();
        }

        /// <summary>
        /// Checks whether the given interval is contained in the collection.
        /// </summary>
        /// <param name="interval">The interval to check for.</param>
        /// <returns><c>true</c> if <paramref name="interval"/> is contained in the collection. Otherwise <c>false</c>.</returns>
        public bool Contains(IInterval<T> interval)
        {
            return _intervals.BinarySearch(interval) >= 0;
        }

        /// <summary>
        /// Copies all intervals into the given array.
        /// </summary>
        /// <param name="array">The array to copy the intervals into.</param>
        /// <param name="arrayIndex">The start position in the destination array.</param>
        public void CopyTo(IInterval<T>[] array, int arrayIndex)
        {
            _intervals.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the given interval from the collection.
        /// </summary>
        /// <param name="interval">The interval to remove.</param>
        /// <returns><c>true</c> if <paramref name="interval"/> was contained in the collection and has been removed. Otherwise <c>false</c>.</returns>
        public bool Remove(IInterval<T> interval)
        {
            var index = _intervals.BinarySearch(interval);
            if (index < 0)
                return false;

            _intervals.RemoveAt(index);
            return true;
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
        /// <returns>An interval collection containing all intervals which lay in the search interval.</returns>
        public IntervalCollection<T> GetBetween(T start, T end)
        {
            var index = BinarySearch(_intervals, SearchBetweenComparator, start, end);
            return index < 0
                 ? new IntervalCollection<T>()
                 : GetLeftAndRight(index, SearchBetweenComparator, start, end);
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
        /// <returns>An interval collection containing all intervals which overlap with the search interval.</returns>
        public IntervalCollection<T> GetOverlapping(T start, T end)
        {
            var index = BinarySearch(_intervals, SearchOverlappingComparator, start, end);
            return index < 0
                 ? new IntervalCollection<T>()
                 : GetLeftAndRight(index, SearchOverlappingComparator, start, end);
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
        /// <returns>An interval collection containing all intervals which overlap with the search interval.</returns>
        public IntervalCollection<T> GetEnclosing(T start, T end)
        {
            var index = BinarySearch(_intervals, SearchEnclosingComparator, start, end);
            return index < 0
                 ? new IntervalCollection<T>()
                 : GetLeftAndRight(index, SearchEnclosingComparator, start, end);
        }

        /// <summary>
        /// Returns an enumerator which iterates through the collection.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<IInterval<T>> GetEnumerator()
        {
            return _intervals.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator which iterates through the collection.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _intervals.GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IntervalCollection<T> GetLeftAndRight(int index, Comparator comparator, T start, T end)
        {
            var from = index;
            for (var i = from - 1; i >= 0 && comparator(_intervals[i], start, end) == 0; i--)
                from = i;

            var to = index + 1;
            for (; to < _intervals.Count && comparator(_intervals[to], start, end) == 0; to++)
                ;

            return IntervalCollection<T>.FromOrdered(_intervals.GetRange(from, to - from));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SearchInsertIndexComparator(IInterval<T> interval, T start, T end)
        {
            var startComparison = interval.Start.CompareTo(start);
            var endComparison = interval.End.CompareTo(end);

            if (startComparison > 0 || startComparison == 0 && endComparison > 0)
                return -1;
            else if (startComparison < 0 || startComparison == 0 && endComparison < 0)
                return 1;
            else
                return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SearchBetweenComparator(IInterval<T> interval, T start, T end)
        {
            var startComparison = interval.Start.CompareTo(start);
            var endComparison = interval.End.CompareTo(end);

            if (startComparison < 0)
                return 1;
            else if (endComparison > 0)
                return -1;
            else
                return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SearchOverlappingComparator(IInterval<T> interval, T start, T end)
        {
            var startComparison = interval.Start.CompareTo(end);
            var endComparison = interval.End.CompareTo(start);

            if (startComparison <= 0 && endComparison >= 0)
                return 0;
            else if (endComparison < 0)
                return 1;
            else
                return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SearchEnclosingComparator(IInterval<T> interval, T start, T end)
        {
            var startComparison = interval.Start.CompareTo(start);
            var endComparison = interval.End.CompareTo(end);

            if (startComparison <= 0 && endComparison >= 0)
                return 0;
            else if (endComparison <= 0)
                return 1;
            else
                return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BinarySearch(List<IInterval<T>> intervals, Comparator comparator, T start, T end)
        {
            var min = 0;
            var max = intervals.Count - 1;
            while (min <= max)
            {
                var mid = (min + max) / 2;

                var comparison = comparator(intervals[mid], start, end);
                if (comparison == 0)
                    return mid;
                else if (comparison < 0)
                    max = mid - 1;
                else
                    min = mid + 1;
            }

            return ~min;
        }
    }
}
