using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilTests
{
    [TestClass]
    public class MultiDictionaryExtensionsTests
    {
        #region Add

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MultiDictionaryExtensions_Add_DictionaryIsNull_ShouldThrowArgumentNullException()
        {
            Dictionary<int, HashSet<string>> dict = null;

            MultiDictionaryExtensions.Add(dict, 0, "Test 0");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MultiDictionaryExtensions_Add_KeyIsNull_ShouldThrowArgumentNullException()
        {
            var dict = new Dictionary<object, HashSet<string>>();
            dict.Add(null, "Test 0");
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Add_ValueIsNull_ShouldAddValue()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            string value = null;
            dict.Add(0, value);

            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual(1, dict[0].Count);
            Assert.IsNull(dict[0].First());
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Add_SingleKeySingleValue_ShouldContainSingleKey()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            dict.Add(0, "Test 0");

            Assert.AreEqual(1, dict[0].Count);
            Assert.IsTrue(dict[0].Contains("Test 0"));
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Add_SingleKeySingleValue_ShouldContainSingleValue()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            dict.Add(0, "Test 0");

            Assert.AreEqual(1, dict[0].Count);
            Assert.IsTrue(dict[0].Contains("Test 0"));
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Add_SingleKeyMultipleValues_ShouldContainSingleKey()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            dict.Add(0, "Test 0");
            dict.Add(0, "Test 1");
            dict.Add(0, "Test 2");

            Assert.AreEqual(1, dict.Count);
            Assert.IsTrue(dict.ContainsKey(0));
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Add_SingleKeyMultipleValues_ShouldContainAllValues()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            dict.Add(0, "Test 0");
            dict.Add(0, "Test 1");
            dict.Add(0, "Test 2");

            Assert.AreEqual(3, dict[0].Count);
            Assert.IsTrue(dict[0].Contains("Test 0"));
            Assert.IsTrue(dict[0].Contains("Test 1"));
            Assert.IsTrue(dict[0].Contains("Test 2"));
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Add_MultipleKeysMultipleValues_ShouldContainAllKeys()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            dict.Add(0, "Test 0");
            dict.Add(0, "Test 1");

            dict.Add(1, "Test 2");

            dict.Add(2, "Test 3");
            dict.Add(2, "Test 4");
            dict.Add(2, "Test 5");

            Assert.AreEqual(3, dict.Count);
            Assert.IsTrue(dict.ContainsKey(0));
            Assert.IsTrue(dict.ContainsKey(1));
            Assert.IsTrue(dict.ContainsKey(2));
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Add_MultipleKeysMultipleValues_ShouldContainAllValues()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            dict.Add(0, "Test 0");
            dict.Add(0, "Test 1");

            dict.Add(1, "Test 2");

            dict.Add(2, "Test 3");
            dict.Add(2, "Test 4");
            dict.Add(2, "Test 5");

            Assert.AreEqual(2, dict[0].Count);
            Assert.IsTrue(dict[0].Contains("Test 0"));
            Assert.IsTrue(dict[0].Contains("Test 1"));

            Assert.AreEqual(1, dict[1].Count);
            Assert.IsTrue(dict[1].Contains("Test 2"));

            Assert.AreEqual(3, dict[2].Count);
            Assert.IsTrue(dict[2].Contains("Test 3"));
            Assert.IsTrue(dict[2].Contains("Test 4"));
            Assert.IsTrue(dict[2].Contains("Test 5"));
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Add_UsingInitializerList_ShouldAddValues()
        {
            var dict = new Dictionary<int, HashSet<string>> {
                { 0, "Test 0" },
                { 0, "Test 1" },
                { 0, "Test 2" }
            };

            Assert.IsTrue(dict[0].Contains("Test 0"));
            Assert.IsTrue(dict[0].Contains("Test 1"));
            Assert.IsTrue(dict[0].Contains("Test 2"));
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Add_CollectionIsList_ShouldAddLists()
        {
            var dict = new Dictionary<int, List<string>> {
                { 0, "Test 0" },
                { 1, "Test 1" }
            };

            Assert.IsInstanceOfType(dict[0], typeof(List<string>));
            Assert.IsInstanceOfType(dict[1], typeof(List<string>));
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Add_CollectionIsHasSet_ShouldAddHashSets()
        {
            var dict = new Dictionary<int, HashSet<string>> {
                { 0, "Test 0" },
                { 1, "Test 1" }
            };

            Assert.IsInstanceOfType(dict[0], typeof(HashSet<string>));
            Assert.IsInstanceOfType(dict[1], typeof(HashSet<string>));
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Add_CollectionIsSortedList_ShouldAddSortedLists()
        {
            var dict = new Dictionary<int, SortedList<int, string>> {
                { 0, KeyValuePair.Create(0, "Test 0") },
                { 1, KeyValuePair.Create(0, "Test 1") }
            };

            Assert.IsInstanceOfType(dict[0], typeof(SortedList<int, string>));
            Assert.IsInstanceOfType(dict[1], typeof(SortedList<int, string>));
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Add_CollectionIsLinkedList_ShouldAddLinkedLists()
        {
            var dict = new Dictionary<int, LinkedList<string>> {
                { 0, "Test 0" },
                { 1, "Test 1" }
            };

            Assert.IsInstanceOfType(dict[0], typeof(LinkedList<string>));
            Assert.IsInstanceOfType(dict[1], typeof(LinkedList<string>));
        }

        #endregion

        #region Remove

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MultiDictionaryExtensions_Remove_DictionaryIsNull_ShouldThrowArgumentNullException()
        {
            Dictionary<int, HashSet<string>> dict = null;

            MultiDictionaryExtensions.Remove(dict, 0, "Test 0");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MultiDictionaryExtensions_Remove_KeyIsNull_ShouldThrowArgumentNullException()
        {
            var dict = new Dictionary<object, HashSet<string>>();
            dict.Remove(null, "Test 0");
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Remove_KeyDoesNotExist_ShouldReturnFalse()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            var result = dict.Remove(1, "Test 0");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Remove_ValueDoesNotExist_ShouldReturnFalse()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            dict.Add(0, "Test 0");

            var result = dict.Remove(0, "Unknown");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Remove_ValueIsNull_ShouldRemoveValue()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            string value = null;
            dict.Add(0, value);

            dict.Remove(0, value);

            Assert.AreEqual(0, dict.Count);
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Remove_OtherValuesRemainInKey_ShouldOnlyRemoveSpecifiedValue()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            dict.Add(0, "Test 0");
            dict.Add(0, "Test 1");
            dict.Add(0, "Test 2");

            dict.Remove(0, "Test 1");

            Assert.AreEqual(2, dict[0].Count);
            Assert.IsTrue(dict[0].Contains("Test 0"));
            Assert.IsTrue(dict[0].Contains("Test 2"));
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Remove_NoValuesRemainInKey_ShouldRemoveKey()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            dict.Add(0, "Test 0");
            dict.Add(0, "Test 1");
            dict.Add(0, "Test 2");

            dict.Remove(0, "Test 0");
            dict.Remove(0, "Test 1");
            dict.Remove(0, "Test 2");

            Assert.AreEqual(0, dict.Count);
        }

        [TestMethod]
        public void MultiDictionaryExtensions_Remove_NoValuesRemainInKey_ShouldNotRemoveOtherKeys()
        {
            var dict = new Dictionary<int, HashSet<string>>();
            dict.Add(0, "Test 0");
            dict.Add(1, "Test 1");
            dict.Add(2, "Test 2");

            dict.Remove(1, "Test 1");

            Assert.AreEqual(2, dict.Count);
        }

        #endregion
    }
}
