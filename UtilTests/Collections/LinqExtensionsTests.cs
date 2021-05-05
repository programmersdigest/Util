using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using programmersdigest.Util.Collections;
using System.Linq;
using System;

namespace UtilTests.Collections
{
    [TestClass]
    public class LinqExtensionsTests
    {
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void LinqExtensions_Scan_CollectionIsNull_ShouldThrowNullReferenceException()
        {
            List<int> collection = null!;
            collection.Scan((state, item) => state + item, 0).ToList();
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void LinqExtensions_Scan_AggregateFunctionIsNull_ShouldThrowNullReferenceException()
        {
            var collection = new[] { 1, 2, 3, 4, 5 };
            collection.Scan(null!, 0).ToList();
        }

        [TestMethod]
        public void LinqExtensions_Scan_CollectionIsEmpty_ShouldReturnEmptyCollection()
        {
            var collection = new int[] { };
            var result = collection.Scan((state, item) => state + item, 0).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void LinqExtensions_Scan_AddIntegers_ShouldReturnAddedValues()
        {
            var collection = new[] { 1, 2 };
            var result = collection.Scan((state, item) => state + item, 0).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(3, result[1]);
        }

        [TestMethod]
        public void LinqExtensions_Scan_NoInitialState_AddIntegers_ShouldBehaveAsIfInitialStateIs0()
        {
            var collection = new[] { 1, 2 };
            var result = collection.Scan<int, int>((state, item) => state + item).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(3, result[1]);
        }

        [TestMethod]
        public void LinqExtensions_Scan_SubtractIntegers_ShouldReturnSubtractedValues()
        {
            var collection = new[] { 1, 2 };
            var result = collection.Scan((state, item) => state - item, 0).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(-1, result[0]);
            Assert.AreEqual(-3, result[1]);
        }

        [TestMethod]
        public void LinqExtensions_Scan_AddIntegers_InitialStateIs5_ShouldReturnAddedValues()
        {
            var collection = new[] { 10, 15 };
            var result = collection.Scan((state, item) => state + item, 5).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(15, result[0]);
            Assert.AreEqual(30, result[1]);
        }

        [TestMethod]
        public void LinqExtensions_Scan_ConcatStrings_ShouldReturnConcatenatedStrings()
        {
            var collection = new[] { "This", "is", "a", "sentence" };
            var result = collection.Scan((state, item) => state + item, "").ToList();

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("This", result[0]);
            Assert.AreEqual("Thisis", result[1]);
            Assert.AreEqual("Thisisa", result[2]);
            Assert.AreEqual("Thisisasentence", result[3]);
        }

        [TestMethod]
        public void LinqExtensions_Scan_ConcatStrings_InitialStateNotGiven_ShouldReturnConcatenatedStrings()
        {
            var collection = new[] { "This", "is", "a", "sentence" };
            var result = collection.Scan<string, string>((state, item) => state + item).ToList();

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("This", result[0]);
            Assert.AreEqual("Thisis", result[1]);
            Assert.AreEqual("Thisisa", result[2]);
            Assert.AreEqual("Thisisasentence", result[3]);
        }
    }
}
