using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace programmersdigest.Util.Collections
{
    /// <summary>
    /// An ObservableCollection with added filtering support. Holds an unfiltered and a filtered collection
    /// so changing the filter function on the fly is possible.
    /// </summary>
    /// <typeparam name="T">The type of the items contained in this collection.</typeparam>
    public class FilteredObservableCollection<T> : ICollection<T>, IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly ICollection<T> _unfilteredCollection = new List<T>();
        private readonly ICollection<T> _filteredCollection = new List<T>();
        private Func<T, bool> _filter = item => true;

        /// <summary>
        /// The number of items contained in this collection. Only unfiltered items are taken into account.
        /// </summary>
        public int Count => _filteredCollection.Count;

        /// <summary>
        /// Always <c>false</c>.
        /// </summary>
        public bool IsReadOnly { get; } = false;

        /// <summary>
        /// The filter function to apply to all items in this collection. Items for which the filter function
        /// returns <c>false</c> are discarded. They wont show up when enumerating over the collection, they
        /// do not get counted, they do not raise CollectionChanged events.
        /// Raises the CollectionChanged event with a <see cref="NotifyCollectionChangedAction.Reset"/>.
        /// </summary>
        public Func<T, bool> Filter
        {
            get => _filter;
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Filter)));
                    ReapplyFilter();
                }
            }
        }

        /// <summary>
        /// Raised when the collection is changed. For specific behaviour check the methods in question.
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// Raised when a property of the collection changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Re-applies the filter function to all items. Should not have to be called.
        /// Raises the CollectionChanged event with a <see cref="NotifyCollectionChangedAction.Reset"/>.
        /// </summary>
        public void ReapplyFilter()
        {
            var count = Count;

            _filteredCollection.Clear();
            foreach (var item in _unfilteredCollection)
            {
                if (_filter(item))
                    _filteredCollection.Add(item);
            }

            if (Count != count)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Adds an item to collection.
        /// If the item is not filtered by <see cref="Filter"/>, <see cref="Count"/> is increased and
        /// a CollectionChanged event with a <see cref="NotifyCollectionChangedAction.Add"/> is raised.
        /// </summary>
        /// <param name="item">The item to add to the collection.</param>
        public void Add(T item)
        {
            _unfilteredCollection.Add(item);

            if (Filter(item))
            {
                _filteredCollection.Add(item);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
        }

        /// <summary>
        /// Removes an item from the collection.
        /// If the item was not filtered by <see cref="Filter"/>, <see cref="Count"/> is decreased and
        /// a CollectionChanged event with a <see cref="NotifyCollectionChangedAction.Remove"/> is raised.
        /// </summary>
        /// <param name="item">The item to add to the collection.</param>
        public bool Remove(T item)
        {
            _unfilteredCollection.Remove(item);

            if (_filteredCollection.Remove(item))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all items from the collection.
        /// <see cref="Count"/> is set to 0 and a CollectionChanged event with a
        /// <see cref="NotifyCollectionChangedAction.Reset"/> is raised.
        /// </summary>
        public void Clear()
        {
            _unfilteredCollection.Clear();
            _filteredCollection.Clear();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Checks whether <paramref name="item"/> is contained in the filtered collection.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        public bool Contains(T item) => _filteredCollection.Contains(item);

        /// <summary>
        /// Copies the filtered collection to the given array.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index ar which to insert items into <paramref name="array"/>.</param>
        public void CopyTo(T[] array, int arrayIndex) => _filteredCollection.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an enumerator over the filtered collection.
        /// </summary>
        public IEnumerator<T> GetEnumerator() => _filteredCollection.GetEnumerator();

        /// <summary>
        /// Returns an enumerator over the filtered collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => _filteredCollection.GetEnumerator();
    }
}
