using System;
using System.Diagnostics;
using System.Linq;
using programmersdigest.Util.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace programmersdigest.UtilTests.Collections
{
    [TestClass]
    [Ignore]
    public class IntervalTreePerformanceTests
    {
        [TestMethod]
        public void Ctor_PerformanceTest()
        {
            const int itemCount = 100000;
            var random = new Random(42);

            var items = Enumerable.Range(0, itemCount).Select(i =>
            {
                var start = random.Next(1000);
                var end = start + random.Next(1, 50);
                return new TestInterval(start, end);
            });

            var stopwatch = Stopwatch.StartNew();

            var collection = new IntervalTree<int>(items, IntervalTreeMedianComputations.IntMedian);

            stopwatch.Stop();

            Assert.Inconclusive($"Constructed with {itemCount} items in {stopwatch.Elapsed}");
        }

        [TestMethod]
        public void GetBetween_PerformanceTest()
        {
            const int itemCount = 100000;
            const int requestCount = 10000;
            var random = new Random(42);

            var items = Enumerable.Range(0, itemCount).Select(i =>
            {
                var start = random.Next(1000);
                var end = start + random.Next(1, 50);
                return new TestInterval(start, end);
            });

            var collection = new IntervalTree<int>(items, IntervalTreeMedianComputations.IntMedian);

            var stopwatch = Stopwatch.StartNew();
            var selectedCount = 0;

            for (var i = 0; i < requestCount; i++)
            {
                var start = random.Next(1000);
                var end = start + random.Next(1, 50);

                var result = collection.GetBetween(start, end).ToList();
                selectedCount += result.Count;
            }

            stopwatch.Stop();

            Assert.Inconclusive($"Executed {nameof(collection.GetBetween)}() {requestCount} times to select {selectedCount} results from {itemCount} items in {stopwatch.Elapsed}");
        }

        [TestMethod]
        public void GetOverlapping_PerformanceTest()
        {
            const int itemCount = 100000;
            const int requestCount = 10000;
            var random = new Random(42);

            var items = Enumerable.Range(0, itemCount).Select(i =>
            {
                var start = random.Next(1000);
                var end = start + random.Next(1, 50);
                return new TestInterval(start, end);
            });

            var collection = new IntervalTree<int>(items, IntervalTreeMedianComputations.IntMedian);

            var stopwatch = Stopwatch.StartNew();
            var selectedCount = 0;

            for (var i = 0; i < requestCount; i++)
            {
                var start = random.Next(1000);
                var end = start + random.Next(1, 50);

                var result = collection.GetOverlapping(start, end).ToList();
                selectedCount += result.Count;
            }

            stopwatch.Stop();

            Assert.Inconclusive($"Executed {nameof(collection.GetOverlapping)}() {requestCount} times to select {selectedCount} results from {itemCount} items in {stopwatch.Elapsed}");
        }

        [TestMethod]
        public void GetEnclosing_PerformanceTest()
        {
            const int itemCount = 100000;
            const int requestCount = 10000;
            var random = new Random(42);

            var items = Enumerable.Range(0, itemCount).Select(i =>
            {
                var start = random.Next(1000);
                var end = start + random.Next(1, 50);
                return new TestInterval(start, end);
            });

            var collection = new IntervalTree<int>(items, IntervalTreeMedianComputations.IntMedian);

            var stopwatch = Stopwatch.StartNew();
            var selectedCount = 0;

            for (var i = 0; i < requestCount; i++)
            {
                var start = random.Next(1000);
                var end = start + random.Next(1, 50);

                var result = collection.GetEnclosing(start, end).ToList();
                selectedCount += result.Count;
            }

            stopwatch.Stop();

            Assert.Inconclusive($"Executed {nameof(collection.GetOverlapping)}() {requestCount} times to select {selectedCount} results from {itemCount} items in {stopwatch.Elapsed}");
        }
    }
}
