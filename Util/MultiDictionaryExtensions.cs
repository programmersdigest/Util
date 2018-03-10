using System;
using System.Collections.Generic;

namespace programmersdigest.Util {
    public static class MultiDictionaryExtensions {
        public static void Add<TKey, TValue, TCollection>(this IDictionary<TKey, TCollection> self, TKey key, TValue value) where TCollection : class, ICollection<TValue> {
            if (self == null) {
                throw new ArgumentNullException(nameof(self), "Dictionary must not be null");
            }

            if (!self.TryGetValue(key, out var collection)) {
                collection = Activator.CreateInstance<TCollection>();
                self.Add(key, collection);
            }

            collection.Add(value);
        }

        public static bool Remove<TKey, TValue, TCollection>(this IDictionary<TKey, TCollection> self, TKey key, TValue value) where TCollection : class, ICollection<TValue> {
            if (self == null) {
                throw new ArgumentNullException(nameof(self), "Dictionary must not be null");
            }

            var success = false;

            if (self.TryGetValue(key, out var collection)) {
                success = collection.Remove(value);

                if (collection.Count <= 0) {
                    self.Remove(key);
                }
            }

            return success;
        }
    }
}
