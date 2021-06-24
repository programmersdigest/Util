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
    public sealed class IntervalCollection<T> : ICollection<IInterval<T>>, IEnumerable<IInterval<T>> where T : IComparable
    {
        private delegate int Comparator(IInterval<T> interval, T start, T end);

        public static IntervalCollection<T> FromOrdered(IEnumerable<IInterval<T>> intervals)
        {
            return new IntervalCollection<T>(intervals.ToList());
        }

        public static IntervalCollection<T> FromUnordered(IEnumerable<IInterval<T>> intervals)
        {
            intervals = intervals.OrderBy(i => i.Start).ThenBy(i => i.End);
            return new IntervalCollection<T>(intervals.ToList());
        }

        private readonly List<IInterval<T>> _intervals;

        public int Count => _intervals.Count;

        public bool IsReadOnly => false;

        public IInterval<T> this[int index] => _intervals[index];

        public IntervalCollection() : this(new List<IInterval<T>>())
        {
        }

        private IntervalCollection(List<IInterval<T>> intervals)
        {
            _intervals = intervals;
        }

        public void Add(IInterval<T> interval)
        {
            var index = BinarySearch(_intervals, SearchInsertIndexComparator, interval.Start, interval.End);
            if (index < 0)
                index = ~index;

            _intervals.Insert(index, interval);
        }

        public void AddRange(IEnumerable<IInterval<T>> intervals)
        {
            foreach (var interval in intervals)
                Add(interval);
        }

        public void Clear()
        {
            _intervals.Clear();
        }

        public bool Contains(IInterval<T> item)
        {
            return _intervals.BinarySearch(item) >= 0;
        }

        public void CopyTo(IInterval<T>[] array, int arrayIndex)
        {
            _intervals.CopyTo(array, arrayIndex);
        }

        public bool Remove(IInterval<T> item)
        {
            var index = _intervals.BinarySearch(item);
            if (index < 0)
                return false;

            _intervals.RemoveAt(index);
            return true;
        }

        public IntervalCollection<T> GetBetween(T start, T end)
        {
            var index = BinarySearch(_intervals, SearchBetweenComparator, start, end);
            return index < 0
                 ? new IntervalCollection<T>()
                 : GetLeftAndRight(index, SearchBetweenComparator, start, end);
        }

        public IntervalCollection<T> GetOverlapping(T start, T end)
        {
            var index = BinarySearch(_intervals, SearchOverlappingComparator, start, end);
            return index < 0
                 ? new IntervalCollection<T>()
                 : GetLeftAndRight(index, SearchOverlappingComparator, start, end);
        }

        public IntervalCollection<T> GetEnclosing(T start, T end)
        {
            var index = BinarySearch(_intervals, SearchEnclosingComparator, start, end);
            return index < 0
                 ? new IntervalCollection<T>()
                 : GetLeftAndRight(index, SearchEnclosingComparator, start, end);
        }

        public IEnumerator<IInterval<T>> GetEnumerator()
        {
            return _intervals.GetEnumerator();
        }

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
