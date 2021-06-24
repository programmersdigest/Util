using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace programmersdigest.Util.Collections
{
    public sealed class IntervalTreeNode<T> where T : IComparable
    {
        public T Median { get; }
        public T IntervalMin { get; }
        public T IntervalMax { get; }
        public T OverlappingMin => OverlappingByStart is null ? Median : OverlappingByStart.First().Start;
        public T OverlappingMax => OverlappingByEnd is null ? Median : OverlappingByEnd.Last().End;
        public List<IInterval<T>>? OverlappingByStart { get; }
        public List<IInterval<T>>? OverlappingByEnd { get; }
        public IntervalTreeNode<T>? Left { get; }
        public IntervalTreeNode<T>? Right { get; }

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
