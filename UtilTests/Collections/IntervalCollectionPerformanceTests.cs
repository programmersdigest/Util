using System;
using System.Diagnostics;
using System.Linq;
using programmersdigest.Util.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace programmersdigest.UtilTests.Collections
{
    [TestClass]
    [Ignore]
    public class IntervalCollectionPerformanceTests
    {
        [TestMethod]
        public void FromOrdered_PerformanceTest()
        {
            const int itemCount = 100000;
            var random = new Random(42);

            int start;
            int end;
            var items = Enumerable.Range(0, itemCount).Select(i =>
            {
                start = random.Next(1000);
                end = start + random.Next(1, 50);
                return new TestInterval(start, end);
            }).OrderBy(i => i.Start).ThenBy(i => i.End).ToList();

            var stopwatch = Stopwatch.StartNew();
            var collection = IntervalCollection<int>.FromOrdered(items);
            stopwatch.Stop();

            Assert.Inconclusive($"Added {itemCount} items in {stopwatch.Elapsed}");
        }

        [TestMethod]
        public void FromUnordered_PerformanceTest()
        {
            const int itemCount = 100000;
            var random = new Random(42);

            int start;
            int end;
            var items = Enumerable.Range(0, itemCount).Select(i =>
            {
                start = random.Next(1000);
                end = start + random.Next(1, 50);
                return new TestInterval(start, end);
            }).ToList();

            var stopwatch = Stopwatch.StartNew();
            var collection = IntervalCollection<int>.FromUnordered(items);
            stopwatch.Stop();

            Assert.Inconclusive($"Added {itemCount} items in {stopwatch.Elapsed}");
        }

        [TestMethod]
        public void Add_PerformanceTest()
        {
            const int itemCount = 100000;
            var random = new Random(42);

            var stopwatch = Stopwatch.StartNew();

            var collection = new IntervalCollection<int>();
            int start;
            int end;
            for (var i = 0; i < itemCount; i++)
            {
                start = random.Next(1000);
                end = start + random.Next(1, 50);
                collection.Add(new TestInterval(start, end));
            }

            stopwatch.Stop();

            Assert.Inconclusive($"Added {itemCount} items in {stopwatch.Elapsed}");
        }

        [TestMethod]
        public void GetBetween_PerformanceTest()
        {
            const int itemCount = 100000;
            const int requestCount = 10000;
            var random = new Random(42);

            var collection = new IntervalCollection<int>();

            {
                int start;
                int end;
                for (var i = 0; i < itemCount; i++)
                {
                    start = random.Next(1000);
                    end = start + random.Next(1, 50);
                    collection.Add(new TestInterval(start, end));
                }
            }

            var stopwatch = Stopwatch.StartNew();
            var selectedCount = 0;

            for (var i = 0; i < requestCount; i++)
            {
                var start = random.Next(1000);
                var end = start + random.Next(50);

                var result = collection.GetBetween(start, end);
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

            var collection = new IntervalCollection<int>();

            {
                int start;
                int end;
                for (var i = 0; i < itemCount; i++)
                {
                    start = random.Next(1000);
                    end = start + random.Next(1, 50);
                    collection.Add(new TestInterval(start, end));
                }
            }

            var stopwatch = Stopwatch.StartNew();
            var selectedCount = 0;

            for (var i = 0; i < requestCount; i++)
            {
                var start = random.Next(1000);
                var end = start + random.Next(50);

                var result = collection.GetOverlapping(start, end);
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

            var collection = new IntervalCollection<int>();

            {
                int start;
                int end;
                for (var i = 0; i < itemCount; i++)
                {
                    start = random.Next(1000);
                    end = start + random.Next(1, 50);
                    collection.Add(new TestInterval(start, end));
                }
            }

            var stopwatch = Stopwatch.StartNew();
            var selectedCount = 0;

            for (var i = 0; i < requestCount; i++)
            {
                var start = random.Next(1000);
                var end = start + random.Next(50);

                var result = collection.GetEnclosing(start, end);
                selectedCount += result.Count;
            }

            stopwatch.Stop();

            Assert.Inconclusive($"Executed {nameof(collection.GetEnclosing)}() {requestCount} times to select {selectedCount} results from {itemCount} items in {stopwatch.Elapsed}");
        }
    }
}
