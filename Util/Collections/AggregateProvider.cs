using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace programmersdigest.Util.Collections
{
    /// <summary>
    /// Computes an aggregate over a collection using the provided aggregation function. Raises a NotifyPropertyChanged
    /// event on the result whenever the collection changed.
    /// </summary>
    /// <typeparam name="TCollection">The collection to accumulate item values of.</typeparam>
    /// <typeparam name="TAccumulate">Type of the resulting accumulated value.</typeparam>
    /// <typeparam name="TItem">Type of the items in the collection.</typeparam>
    public class AggregateProvider<TCollection, TAccumulate, TItem> : INotifyPropertyChanged, IDisposable where TCollection : INotifyCollectionChanged, IEnumerable<TItem>
    {
        private readonly TCollection _sourceCollection;
        private readonly Func<TAccumulate, TItem, TAccumulate> _aggregationFunction;
        private readonly TAccumulate _initialValue;
        private TAccumulate _value = default!;

        /// <summary>
        /// Raised whenever the computed <see cref="Value"/> changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// The accumulated value computed using the aggregation function on each item in the source collection.
        /// </summary>
        public TAccumulate Value
        {
            get => _value;
            private set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AggregateProvider{TCollection, TAccumulate, TItem}" /> class.
        /// </summary>
        /// <param name="sourceCollection">The collection over whose items to compute the accumulated <see cref="Value"/>.</param>
        /// <param name="aggregationFunction">The aggregation function to apply to each item in <paramref name="sourceCollection"/>.</param>
        /// <param name="initialValue">The initial value of the accumulated <see cref="Value"/>.</param>
        public AggregateProvider(TCollection sourceCollection, Func<TAccumulate, TItem, TAccumulate> aggregationFunction, TAccumulate initialValue)
        {
            _sourceCollection = sourceCollection ?? throw new ArgumentNullException(nameof(sourceCollection));
            _aggregationFunction = aggregationFunction ?? throw new ArgumentNullException(nameof(aggregationFunction));
            _initialValue = initialValue ?? throw new ArgumentNullException(nameof(initialValue));

            _sourceCollection.CollectionChanged += SourceCollectionChanged;
            Recalculate();
        }

        /// <summary>
        /// Releases the CollectionChanged event from the source collection.
        /// </summary>
        public void Dispose()
        {
            _sourceCollection.CollectionChanged -= SourceCollectionChanged;
        }

        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Recalculate();
        }

        private void Recalculate()
        {
            Value = _sourceCollection.Aggregate(_initialValue, _aggregationFunction);
        }
    }
}
