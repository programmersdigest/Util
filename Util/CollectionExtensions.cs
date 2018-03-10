using System;
using System.Collections.Generic;

namespace programmersdigest.Util {
    public static class CollectionExtensions {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items) {
            if (collection == null) {
                throw new ArgumentNullException(nameof(collection), "Collection must not be null.");
            }

            if (items == null) {
                throw new ArgumentNullException(nameof(items), "Items enumerable must not be null.");
            }

            foreach (var item in items) {
                collection.Add(item);
            }
        }
    }
}
