using System;
using System.Collections.Generic;

namespace programmersdigest.Util
{
    /// <summary>
    /// Custom extensions to LINQ.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Aggregates all items in the given <paramref name="collection"/> using the given <paramref name="aggregateFunction"/>.
        /// Every iteration the <paramref name="aggregateFunction"/> gets two parameters: the state of the previous iteration and
        /// the current iterations item. After the function completes, it returns the new state which is again provided in the
        /// next iteration.
        /// </summary>
        /// <typeparam name="T">Type of the items in the given <paramref name="collection"/>.</typeparam>
        /// <typeparam name="U">Type of the result of the <paramref name="aggregateFunction"/>.</typeparam>
        /// <param name="collection">The collection over which to iterate for aggregation.</param>
        /// <param name="aggregateFunction">The function to apply to every item in the <paramref name="collection"/>.</param>
        /// <param name="state">The initial state of the aggregation process.</param>
        /// <returns>An enumerable of results of the <paramref name="aggregateFunction"/>.</returns>
        /// <example>
        /// String concatenation:
        /// var collection = new[] { "This", "is", "a", "sentence" };
        /// var result = collection.Scan((state, item) => state + item, "").ToList();
        ///
        /// Assert.AreEqual(4, result.Count);
        /// Assert.AreEqual("This", result[0]);
        /// Assert.AreEqual("Thisis", result[1]);
        /// Assert.AreEqual("Thisisa", result[2]);
        /// Assert.AreEqual("Thisisasentence", result[3]);
        /// 
        /// Integer addition:
        /// var collection = new[] { 1, 2 };
        /// var result = collection.Scan((state, item) => state + item, 0).ToList();
        ///
        /// Assert.AreEqual(2, result.Count);
        /// Assert.AreEqual(1, result[0]);
        /// Assert.AreEqual(3, result[1]);
        /// </example>
        public static IEnumerable<U> Scan<T, U>(this IEnumerable<T> collection, Func<U, T, U> aggregateFunction, U state = default(U))
        {
            foreach (var item in collection)
            {
                state = aggregateFunction(state, item);
                yield return state;
            }
        }
    }
}
