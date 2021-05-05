using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using programmersdigest.Util.Collections;
using System.Linq;
using System.Collections.ObjectModel;

namespace UtilTests.Collections
{
    [TestClass]
    public class CollectionExtensionsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CollectionExtensions_AddRange_CollectionIsNull_ShouldThrowArgumentNullException()
        {
            ICollection<string> collection = null!;
            var items = new string[] { "Test 1" };

            programmersdigest.Util.Collections.CollectionExtensions.AddRange(collection, items);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CollectionExtensions_AddRange_ItemsIsNull_ShouldThrowArgumentNullException()
        {
            var collection = new Collection<string>();
            collection.AddRange(null!);
        }

        [TestMethod]
        public void CollectionExtensions_AddRange_ItemsIsEmpty_ShouldDoNothing()
        {
            var collection = new Collection<string>();
            var items = new string[] { };

            collection.AddRange(items);

            Assert.IsFalse(collection.Any());
        }

        [TestMethod]
        public void CollectionExtensions_AddRange_SingleItem_ShouldAddItem()
        {
            var collection = new Collection<string>();
            var items = new string[] { "Test 1" };

            collection.AddRange(items);

            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual("Test 1", collection[0]);
        }

        [TestMethod]
        public void CollectionExtensions_AddRange_MultipleItems_ShouldAddAllItems()
        {
            var collection = new Collection<string>();
            var items = new string[] { "Test 1", "Test 2", "Test 3" };

            collection.AddRange(items);

            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual("Test 1", collection[0]);
            Assert.AreEqual("Test 2", collection[1]);
            Assert.AreEqual("Test 3", collection[2]);
        }

        [TestMethod]
        public void CollectionExtensions_AddRange_CollectionNotEmpty_MultipleItems_ShouldAddAllItems()
        {
            var collection = new Collection<string> { "Test A", "Test B" };
            var items = new string[] { "Test 1", "Test 2", "Test 3" };

            collection.AddRange(items);

            Assert.AreEqual(5, collection.Count);
            Assert.AreEqual("Test A", collection[0]);
            Assert.AreEqual("Test B", collection[1]);
            Assert.AreEqual("Test 1", collection[2]);
            Assert.AreEqual("Test 2", collection[3]);
            Assert.AreEqual("Test 3", collection[4]);
        }
    }
}
