using System;
using System.Linq;
using programmersdigest.Util.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace programmersdigest.UtilTests.Collections
{
    [TestClass]
    public class IntervalTreeTests
    {
        #region GetBetween()

        [TestMethod]
        public void GetBetween_NoItems_ReturnsEmptyCollection()
        {
            var collection = new IntervalTree<int>(Array.Empty<IInterval<int>>(), IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetBetween(0, 8).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetBetween_NoItemsAreBetween_ReturnsEmptyCollection()
        {
            var collection = new IntervalTree<int>(new TestInterval[] {
                (0, 2), (0, 4), (3, 5)
            }, IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetBetween(6, 8).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetBetween_AllItemsAreBetween_ReturnsAllItems()
        {
            var collection = new IntervalTree<int>(new TestInterval[] {
                (3, 5), (0, 2), (6, 8), (0, 4)
            }, IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetBetween(0, 8).OrderBy(i => i.Start).ThenBy(i => i.End).ToList();

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual(0, result[0].Start);
            Assert.AreEqual(2, result[0].End);

            Assert.AreEqual(0, result[1].Start);
            Assert.AreEqual(4, result[1].End);

            Assert.AreEqual(3, result[2].Start);
            Assert.AreEqual(5, result[2].End);

            Assert.AreEqual(6, result[3].Start);
            Assert.AreEqual(8, result[3].End);
        }

        [TestMethod]
        public void GetBetween_FirstItemsAreBetween_ReturnsCorrectItems()
        {
            var collection = new IntervalTree<int>(new TestInterval[] {
                (3, 5), (0, 2), (6, 8), (0, 4)
            }, IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetBetween(0, 4).OrderBy(i => i.Start).ThenBy(i => i.End).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(0, result[0].Start);
            Assert.AreEqual(2, result[0].End);

            Assert.AreEqual(0, result[1].Start);
            Assert.AreEqual(4, result[1].End);
        }

        [TestMethod]
        public void GetBetween_LastItemsAreBetween_ReturnsCorrectItems()
        {
            var collection = new IntervalTree<int>(new TestInterval[] {
                (3, 5), (0, 2), (6, 8), (0, 4)
            }, IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetBetween(3, 8).OrderBy(i => i.Start).ThenBy(i => i.End).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(3, result[0].Start);
            Assert.AreEqual(5, result[0].End);

            Assert.AreEqual(6, result[1].Start);
            Assert.AreEqual(8, result[1].End);
        }

        [TestMethod]
        [DataRow(1), DataRow(6), DataRow(24), DataRow(80), DataRow(240), DataRow(672), DataRow(1792)]
        public void GetBetween_VariousDataTests(int seed)
        {
            const int itemCount = 1000;
            const int requestCount = 100;
            var random = new Random(seed);

            var items = Enumerable.Range(0, itemCount).Select(i =>
            {
                var start = random.Next(1000);
                var end = start + random.Next(1, 50);
                return new TestInterval(start, end);
            }).ToList();

            var collection = new IntervalTree<int>(items, IntervalTreeMedianComputations.IntMedian);

            for (var i = 0; i < requestCount; i++)
            {
                var start = random.Next(1000);
                var end = start + random.Next(1, 50);

                var result = collection.GetBetween(start, end).ToList();

                var expected = items.Where(i => i.Start >= start && i.End <= end).ToList();
                CollectionAssert.AreEquivalent(expected, result);
            }
        }

        #endregion

        #region GetOverlapping()

        [TestMethod]
        public void GetOverlapping_NoItems_ReturnsEmptyCollection()
        {
            var collection = new IntervalTree<int>(Array.Empty<IInterval<int>>(), IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetOverlapping(0, 8);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetOverlapping_NoItemsAreOverlapping_ReturnsEmptyCollection()
        {
            var collection = new IntervalTree<int>(new TestInterval[] {
                (0, 2), (0, 4), (3, 5)
            }, IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetOverlapping(6, 8);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetOverlapping_AllItemsAreOverlapping_ReturnsAllItems()
        {
            var collection = new IntervalTree<int>(new TestInterval[] {
                (3, 5), (0, 2), (6, 8), (0, 4)
            }, IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetOverlapping(1, 7).OrderBy(i => i.Start).ThenBy(i => i.End).ToList();

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual(0, result[0].Start);
            Assert.AreEqual(2, result[0].End);

            Assert.AreEqual(0, result[1].Start);
            Assert.AreEqual(4, result[1].End);

            Assert.AreEqual(3, result[2].Start);
            Assert.AreEqual(5, result[2].End);

            Assert.AreEqual(6, result[3].Start);
            Assert.AreEqual(8, result[3].End);
        }

        [TestMethod]
        public void GetOverlapping_FirstItemsAreOverlapping_ReturnsCorrectItems()
        {
            var collection = new IntervalTree<int>(new TestInterval[] {
                (3, 5), (0, 2), (6, 8), (0, 4)
            }, IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetOverlapping(1, 3).OrderBy(i => i.Start).ThenBy(i => i.End).ToList();

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(0, result[0].Start);
            Assert.AreEqual(2, result[0].End);

            Assert.AreEqual(0, result[1].Start);
            Assert.AreEqual(4, result[1].End);

            Assert.AreEqual(3, result[2].Start);
            Assert.AreEqual(5, result[2].End);
        }

        [TestMethod]
        public void GetOverlapping_LastItemsAreOverlapping_ReturnsCorrectItems()
        {
            var collection = new IntervalTree<int>(new TestInterval[] {
                (3, 5), (0, 2), (6, 8), (0, 3)
            }, IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetOverlapping(4, 7).OrderBy(i => i.Start).ThenBy(i => i.End).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(3, result[0].Start);
            Assert.AreEqual(5, result[0].End);

            Assert.AreEqual(6, result[1].Start);
            Assert.AreEqual(8, result[1].End);
        }

        [TestMethod]
        public void GetOverlapping_ItemsAreOverlappingOnBothSides_ReturnsOverlappingItems()
        {
            var collection = new IntervalTree<int>(new TestInterval[] {
                (0, 4), (1, 2), (4, 6), (1, 3), (7, 9), (1, 5)
            }, IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetOverlapping(2, 6).OrderBy(i => i.Start).ThenBy(i => i.End).ToList();

            Assert.AreEqual(5, result.Count);

            Assert.AreEqual(0, result[0].Start);
            Assert.AreEqual(4, result[0].End);

            Assert.AreEqual(1, result[1].Start);
            Assert.AreEqual(2, result[1].End);

            Assert.AreEqual(1, result[2].Start);
            Assert.AreEqual(3, result[2].End);

            Assert.AreEqual(1, result[3].Start);
            Assert.AreEqual(5, result[3].End);

            Assert.AreEqual(4, result[4].Start);
            Assert.AreEqual(6, result[4].End);
        }

        [TestMethod]
        [DataRow(1), DataRow(6), DataRow(24), DataRow(80), DataRow(240), DataRow(672), DataRow(1792)]
        public void GetOverlapping_VariousDataTests(int seed)
        {
            const int itemCount = 1000;
            const int requestCount = 100;
            var random = new Random(seed);

            var items = Enumerable.Range(0, itemCount).Select(i =>
            {
                var start = random.Next(1000);
                var end = start + random.Next(1, 50);
                return new TestInterval(start, end);
            }).ToList();

            var collection = new IntervalTree<int>(items, IntervalTreeMedianComputations.IntMedian);

            for (var i = 0; i < requestCount; i++)
            {
                var start = random.Next(1000);
                var end = start + random.Next(1, 50);

                var result = collection.GetOverlapping(start, end).ToList();

                var expected = items.Where(i => i.Start <= end && i.End >= start).ToList();
                CollectionAssert.AreEquivalent(expected, result);
            }
        }

        #endregion

        #region GetEnclosing()

        [TestMethod]
        public void GetEnclosing_NoItems_ReturnsEmptyCollection()
        {
            var collection = new IntervalTree<int>(Array.Empty<IInterval<int>>(), IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetEnclosing(0, 8);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetEnclosing_NoItemsAreEnclosing_ReturnsEmptyCollection()
        {
            var collection = new IntervalTree<int>(new TestInterval[] {
                (0, 2), (0, 4), (3, 5)
            }, IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetEnclosing(1, 5).OrderBy(i => i.Start).ThenBy(i => i.End).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetEnclosing_AllItemsAreEnclosing_ReturnsAllItems()
        {
            var collection = new IntervalTree<int>(new TestInterval[] {
                (0, 8), (1, 7), (2, 8), (3, 6)
            }, IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetEnclosing(3, 6).OrderBy(i => i.Start).ThenBy(i => i.End).ToList();

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual(0, result[0].Start);
            Assert.AreEqual(8, result[0].End);

            Assert.AreEqual(1, result[1].Start);
            Assert.AreEqual(7, result[1].End);

            Assert.AreEqual(2, result[2].Start);
            Assert.AreEqual(8, result[2].End);

            Assert.AreEqual(3, result[3].Start);
            Assert.AreEqual(6, result[3].End);
        }

        [TestMethod]
        public void GetEnclosing_SomeItemsAreEnclosing_ReturnsCorrectItems()
        {
            var collection = new IntervalTree<int>(new TestInterval[] {
                (4, 7), (0, 8), (4, 5), (1, 6), (2, 3), (3, 5), (4, 8), (3, 6), (0, 4)
            }, IntervalTreeMedianComputations.IntMedian);

            var result = collection.GetEnclosing(3, 6).OrderBy(i => i.Start).ThenBy(i => i.End).ToList();

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(0, result[0].Start);
            Assert.AreEqual(8, result[0].End);

            Assert.AreEqual(1, result[1].Start);
            Assert.AreEqual(6, result[1].End);

            Assert.AreEqual(3, result[2].Start);
            Assert.AreEqual(6, result[2].End);
        }

        [TestMethod]
        [DataRow(1), DataRow(6), DataRow(24), DataRow(80), DataRow(240), DataRow(672), DataRow(1792)]
        public void GetEnclosing_VariousDataTests(int seed)
        {
            const int itemCount = 1000;
            const int requestCount = 100;
            var random = new Random(seed);

            var items = Enumerable.Range(0, itemCount).Select(i =>
            {
                var start = random.Next(1000);
                var end = start + random.Next(1, 50);
                return new TestInterval(start, end);
            }).ToList();

            var collection = new IntervalTree<int>(items, IntervalTreeMedianComputations.IntMedian);

            for (var i = 0; i < requestCount; i++)
            {
                var start = random.Next(1000);
                var end = start + random.Next(1, 50);

                var result = collection.GetEnclosing(start, end).ToList();

                var expected = items.Where(i => i.Start <= start && i.End >= end).ToList();
                CollectionAssert.AreEquivalent(expected, result);
            }
        }

        #endregion
    }
}
