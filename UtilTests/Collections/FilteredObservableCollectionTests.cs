using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.Util.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace UtilTests.Collections
{
    [TestClass]
    public class FilteredObservableCollectionTests
    {
        [TestMethod]
        public void FilteredObservableCollection_Add_AddsItem()
        {
            var filteredCollection = new FilteredObservableCollection<string>();
            filteredCollection.Add("Item 1");

            Assert.AreEqual("Item 1", filteredCollection.First());
        }

        [TestMethod]
        public void FilteredObservableCollection_Count_ShowsCorrectItemCount()
        {
            var filteredCollection = new FilteredObservableCollection<string>();
            filteredCollection.Add("Item 1");

            Assert.AreEqual("Item 1", filteredCollection.First());
        }

        [TestMethod]
        public void FilteredObservableCollection_Remove_RemovesItem()
        {
            var filteredCollection = new FilteredObservableCollection<string>();
            filteredCollection.Add("Item 1");
            filteredCollection.Add("Item 2");
            filteredCollection.Add("Item 3");

            Assert.IsTrue(filteredCollection.Remove("Item 2"));
            Assert.AreEqual("Item 1", filteredCollection.First());
            Assert.AreEqual("Item 3", filteredCollection.Last());
        }

        [TestMethod]
        public void FilteredObservableCollection_Clear_RemovesAllItems()
        {
            var filteredCollection = new FilteredObservableCollection<string>();
            filteredCollection.Add("Item 1");
            filteredCollection.Add("Item 2");
            filteredCollection.Add("Item 3");

            filteredCollection.Clear();

            foreach (var item in filteredCollection)
            {
                Assert.Fail("Collection must not contain items after Clear()!");
            }
        }

        [TestMethod]
        public void FilteredObservableCollection_Count_ShowsItemCount()
        {
            var filteredCollection = new FilteredObservableCollection<string>();

            filteredCollection.Add("Item 1");
            Assert.AreEqual(1, filteredCollection.Count);

            filteredCollection.Add("Item 2");
            filteredCollection.Add("Item 3");
            Assert.AreEqual(3, filteredCollection.Count);

            filteredCollection.Remove("Item 2");
            Assert.AreEqual(2, filteredCollection.Count);

            filteredCollection.Clear();
            Assert.AreEqual(0, filteredCollection.Count);
        }

        [TestMethod]
        public void FilteredObservableCollection_WithFilter_ItemsAreFiltered()
        {
            var filteredCollection = new FilteredObservableCollection<int>();
            filteredCollection.Filter = item => item != 13;

            filteredCollection.Add(5);
            filteredCollection.Add(7);
            filteredCollection.Add(13);
            filteredCollection.Add(17);

            Assert.AreEqual(3, filteredCollection.Count);

            Assert.AreEqual(5, filteredCollection.First());
            Assert.AreEqual(7, filteredCollection.ElementAt(1));
            Assert.AreEqual(17, filteredCollection.ElementAt(2));
        }

        #region PropertyChanged

        [TestMethod]
        public void FilteredObservableCollection_FilterChanged_ItemsAreFilteredByNewFilter()
        {
            var filteredCollection = new FilteredObservableCollection<string>();
            filteredCollection.Filter = item => item != "Filter me!";

            filteredCollection.Add("Not me");
            filteredCollection.Add("Nor me");
            filteredCollection.Add("Filter me!");
            filteredCollection.Add("But not me");

            Assert.AreEqual(3, filteredCollection.Count);
            Assert.AreEqual("Not me", filteredCollection.First());
            Assert.AreEqual("Nor me", filteredCollection.ElementAt(1));
            Assert.AreEqual("But not me", filteredCollection.ElementAt(2));

            filteredCollection.Filter = item => !item.StartsWith("No");

            Assert.AreEqual(2, filteredCollection.Count);
            Assert.AreEqual("Filter me!", filteredCollection.First());
            Assert.AreEqual("But not me", filteredCollection.Last());
        }

        [TestMethod]
        public void FilteredObservableCollection_Add_RaisesPropertyChangedOnCountForUnfilteredItems()
        {
            var filteredCollection = new FilteredObservableCollection<int>();
            filteredCollection.Filter = item => item != 13;

            var propertyChangedRaisedCounter = 0;
            void propertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(filteredCollection.Count))
                    propertyChangedRaisedCounter++;
            }

            filteredCollection.PropertyChanged += propertyChanged;

            filteredCollection.Add(5);
            filteredCollection.Add(7);
            filteredCollection.Add(13);
            filteredCollection.Add(17);

            filteredCollection.PropertyChanged -= propertyChanged;

            Assert.AreEqual(3, propertyChangedRaisedCounter);
        }

        [TestMethod]
        public void FilteredObservableCollection_Remove_RaisesPropertyChangedOnCountForUnfilteredItems()
        {
            var filteredCollection = new FilteredObservableCollection<int>();
            filteredCollection.Filter = item => item != 13;

            filteredCollection.Add(5);
            filteredCollection.Add(7);
            filteredCollection.Add(13);
            filteredCollection.Add(17);

            var propertyChangedRaisedCounter = 0;
            void propertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(filteredCollection.Count))
                    propertyChangedRaisedCounter++;
            }

            filteredCollection.PropertyChanged += propertyChanged;

            filteredCollection.Remove(7);
            filteredCollection.Remove(13);
            filteredCollection.Remove(5);

            filteredCollection.PropertyChanged -= propertyChanged;

            Assert.AreEqual(2, propertyChangedRaisedCounter);
        }

        [TestMethod]
        public void FilteredObservableCollection_Clear_RaisesPropertyChanged()
        {
            var filteredCollection = new FilteredObservableCollection<int>();
            filteredCollection.Filter = item => item != 13;

            filteredCollection.Add(5);
            filteredCollection.Add(7);
            filteredCollection.Add(13);
            filteredCollection.Add(17);

            var propertyChangedRaisedCounter = 0;
            void propertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(filteredCollection.Count))
                    propertyChangedRaisedCounter++;
            }

            filteredCollection.PropertyChanged += propertyChanged;

            filteredCollection.Clear();

            filteredCollection.PropertyChanged -= propertyChanged;

            Assert.AreEqual(1, propertyChangedRaisedCounter);
        }

        [TestMethod]
        public void FilteredObservableCollection_FilterChanged_RaisesPropertyChangedOnCountForUnfilteredItems()
        {
            var filteredCollection = new FilteredObservableCollection<int>();
            filteredCollection.Filter = item => item != 13;

            filteredCollection.Add(5);
            filteredCollection.Add(7);
            filteredCollection.Add(13);
            filteredCollection.Add(17);

            var propertyChangedRaisedCounter = 0;
            void propertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(filteredCollection.Count))
                    propertyChangedRaisedCounter++;
            }

            filteredCollection.PropertyChanged += propertyChanged;

            filteredCollection.Filter = item => item != 13 && item != 17;   // Count 1 -> 2: Raises PropertyChanged
            filteredCollection.Filter = item => item != 15 && item != 7;    // Count 2 -> 2: Doea not raise PropertyChanged
            filteredCollection.Filter = item => item != 17;                 // Count 2 -> 1: Raises PropertyChanged

            filteredCollection.PropertyChanged -= propertyChanged;

            Assert.AreEqual(2, propertyChangedRaisedCounter);
        }

        #endregion

        #region CollectionChanged



        [TestMethod]
        public void FilteredObservableCollection_Add_RaisesCollectionChangedForUnfilteredItems()
        {
            var filteredCollection = new FilteredObservableCollection<int>();
            filteredCollection.Filter = item => item != 13;

            var changedItems = new List<int>();
            void collectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    changedItems.AddRange(e.NewItems.Cast<int>());
            }

            filteredCollection.CollectionChanged += collectionChanged;

            filteredCollection.Add(5);
            filteredCollection.Add(7);
            filteredCollection.Add(13);
            filteredCollection.Add(17);

            filteredCollection.CollectionChanged -= collectionChanged;

            Assert.AreEqual(3, changedItems.Count);
            Assert.AreEqual(5, changedItems[0]);
            Assert.AreEqual(7, changedItems[1]);
            Assert.AreEqual(17, changedItems[2]);
        }

        [TestMethod]
        public void FilteredObservableCollection_Remove_RaisesCollectionChangedForUnfilteredItems()
        {
            var filteredCollection = new FilteredObservableCollection<int>();
            filteredCollection.Filter = item => item != 13;

            filteredCollection.Add(5);
            filteredCollection.Add(7);
            filteredCollection.Add(13);
            filteredCollection.Add(17);

            var changedItems = new List<int>();
            void collectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                    changedItems.AddRange(e.OldItems.Cast<int>());
            }

            filteredCollection.CollectionChanged += collectionChanged;

            filteredCollection.Remove(13);
            filteredCollection.Remove(7);
            filteredCollection.Remove(11);      // Not even contained!
            filteredCollection.Remove(17);

            filteredCollection.CollectionChanged -= collectionChanged;

            Assert.AreEqual(2, changedItems.Count);
            Assert.AreEqual(7, changedItems[0]);
            Assert.AreEqual(17, changedItems[1]);
        }

        [TestMethod]
        public void FilteredObservableCollection_Clear_RaisesCollectionChangedForUnfilteredItems()
        {
            var filteredCollection = new FilteredObservableCollection<int>();
            filteredCollection.Filter = item => item != 13;

            filteredCollection.Add(5);
            filteredCollection.Add(7);
            filteredCollection.Add(13);
            filteredCollection.Add(17);

            var collectionChangedRaisedCounter = 0;
            void collectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                    collectionChangedRaisedCounter++;
            }

            filteredCollection.CollectionChanged += collectionChanged;

            filteredCollection.Clear();
            filteredCollection.Clear();     // CollectionChanged raised allthough collection already empty!

            filteredCollection.CollectionChanged -= collectionChanged;

            Assert.AreEqual(2, collectionChangedRaisedCounter);
        }

        [TestMethod]
        public void FilteredObservableCollection_FilterChanged_RaisesCollectionChangedForUnfilteredItems()
        {
            var filteredCollection = new FilteredObservableCollection<int>();
            filteredCollection.Filter = item => item != 13;

            filteredCollection.Add(5);
            filteredCollection.Add(7);
            filteredCollection.Add(13);
            filteredCollection.Add(17);

            var collectionChangedRaisedCounter = 0;
            void collectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                    collectionChangedRaisedCounter++;
            }

            filteredCollection.CollectionChanged += collectionChanged;

            filteredCollection.Filter = item => item != 17;
            filteredCollection.Filter = item => item != 17;
            filteredCollection.Filter = item => item != 13;

            filteredCollection.CollectionChanged -= collectionChanged;

            Assert.AreEqual(3, collectionChangedRaisedCounter);
        }

        #endregion
    }
}
