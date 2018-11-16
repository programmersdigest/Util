using System;
using System.Collections.Generic;

namespace programmersdigest.Util.Collections
{
    /// <summary>
    /// Various Extension methods for collection classes such as <see cref="ICollection{T}"/>.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds the given <paramref name="items"/> to the <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in both collections.</typeparam>
        /// <param name="collection">The collection to add the given items to.</param>
        /// <param name="items">The items to be added to <paramref name="collection"/>.</param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection), "Collection must not be null.");
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items), "Items enumerable must not be null.");
            }

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
