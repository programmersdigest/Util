using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.Util.Collections;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UtilTests.Collections
{
    [TestClass]
    public class AggregateProviderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AggregateProvider_ctor_CollectionIsNull_ShouldThrowArgumentNullException()
        {
            int aggregationFunction(int state, int current) => current + state;

            _ = new AggregateProvider<ObservableCollection<int>, int, int>(null!, aggregationFunction, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AggregateProvider_ctor_AggregationFunctionIsNull_ShouldThrowArgumentNullException()
        {
            var collection = new ObservableCollection<int>();

            _ = new AggregateProvider<ObservableCollection<int>, int, int>(collection, null!, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AggregateProvider_ctor_InitialValueIsNull_ShouldThrowArgumentNullException()
        {
            var collection = new ObservableCollection<string>();
            string aggregationFunction(string state, string current) => current + state;

            _ = new AggregateProvider<ObservableCollection<string>, string, string>(collection, aggregationFunction, null!);
        }

        [TestMethod]
        public void AggregateProvider_Value_CorrectlyComputedAfterInitialization()
        {
            var collection = new ObservableCollection<int>
            {
                3, 5, 7
            };
            int aggregationFunction(int state, int current) => current + state;

            var aggregateProvider = new AggregateProvider<ObservableCollection<int>, int, int>(collection, aggregationFunction, 2);

            Assert.AreEqual(17, aggregateProvider.Value);
        }

        [TestMethod]
        public void AggregateProvider_Value_UpdatedAfterAddingItem()
        {
            var collection = new ObservableCollection<int>
            {
                3, 5, 7
            };
            int aggregationFunction(int state, int current) => current + state;

            var aggregateProvider = new AggregateProvider<ObservableCollection<int>, int, int>(collection, aggregationFunction, 2);

            Assert.AreEqual(17, aggregateProvider.Value);

            collection.Add(11);

            Assert.AreEqual(28, aggregateProvider.Value);
        }

        [TestMethod]
        public void AggregateProvider_Value_UpdatedAfterRemovingItem()
        {
            var collection = new ObservableCollection<int>
            {
                3, 5, 7
            };
            int aggregationFunction(int state, int current) => current + state;

            var aggregateProvider = new AggregateProvider<ObservableCollection<int>, int, int>(collection, aggregationFunction, 2);

            Assert.AreEqual(17, aggregateProvider.Value);

            collection.Remove(5);

            Assert.AreEqual(12, aggregateProvider.Value);
        }

        [TestMethod]
        public void AggregateProvider_Value_UpdatedAfterClearingCollection()
        {
            var collection = new ObservableCollection<int>
            {
                3, 5, 7
            };
            int aggregationFunction(int state, int current) => current + state;

            var aggregateProvider = new AggregateProvider<ObservableCollection<int>, int, int>(collection, aggregationFunction, 2);

            Assert.AreEqual(17, aggregateProvider.Value);

            collection.Clear();

            Assert.AreEqual(2, aggregateProvider.Value);
        }

        [TestMethod]
        public void AggregateProvider_Value_RaisesPropertyChangedOnCollectionChange()
        {
            var collection = new ObservableCollection<int>
            {
                3, 5, 7
            };
            int aggregationFunction(int state, int current) => current + state;

            var aggregateProvider = new AggregateProvider<ObservableCollection<int>, int, int>(collection, aggregationFunction, 2);

            var propertyChangedRaisedCounter = 0;
            void propertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(aggregateProvider.Value))
                    propertyChangedRaisedCounter++;
            }

            aggregateProvider.PropertyChanged += propertyChanged;

            collection.Add(11);
            collection.Remove(5);
            collection.Clear();

            aggregateProvider.PropertyChanged -= propertyChanged;

            Assert.AreEqual(3, propertyChangedRaisedCounter);
        }
    }
}
