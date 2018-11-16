using System;
using System.Collections.Generic;

namespace programmersdigest.Util.Collections
{
    /// <summary>
    /// Extension methods to ease the use of dictionaries mapping a key to a collection of values.
    /// </summary>
    public static class MultiDictionaryExtensions
    {
        /// <summary>
        /// Adds the given <paramref name="value"/> to the collection found under the given <paramref name="key"/>. If
        /// the key does not yet exist, it is added to the dictionary and a new collection is created which contains
        /// only the given <paramref name="value"/>. Otherwise the <paramref name="value"/> gets appended to the
        /// existing collection.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys in <paramref name="self"/>.</typeparam>
        /// <typeparam name="TValue">Type of the values in <paramref name="self"/>.</typeparam>
        /// <typeparam name="TCollection">Type of the collections in which the values are stored.</typeparam>
        /// <param name="self">The dictionary to add the <paramref name="value"/> to.</param>
        /// <param name="key">The key under which to add <paramref name="value"/>.</param>
        /// <param name="value">The value to add to the dictionary.</param>
        public static void Add<TKey, TValue, TCollection>(this IDictionary<TKey, TCollection> self, TKey key, TValue value) where TCollection : class, ICollection<TValue>
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self), "Dictionary must not be null");
            }

            if (!self.TryGetValue(key, out var collection))
            {
                collection = Activator.CreateInstance<TCollection>();
                self.Add(key, collection);
            }

            collection.Add(value);
        }

        /// <summary>
        /// Removes the given <paramref name="value"/> from the collection found under the given <paramref name="key"/>. If
        /// the key does exist and the collection contains the given <paramref name="value"/>, the value gets removed from
        /// the collection (otherwise, no action is performed). If the collection is empty after the value has been removed,
        /// the collection and wit it the key are removed entirely from the dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys in <paramref name="self"/>.</typeparam>
        /// <typeparam name="TValue">Type of the values in <paramref name="self"/>.</typeparam>
        /// <typeparam name="TCollection">Type of the collections in which the values are stored.</typeparam>
        /// <param name="self">The dictionary to remove the <paramref name="value"/> from.</param>
        /// <param name="key">The key from which to remove <paramref name="value"/>.</param>
        /// <param name="value">The value to remove from the dictionary.</param>
        public static bool Remove<TKey, TValue, TCollection>(this IDictionary<TKey, TCollection> self, TKey key, TValue value) where TCollection : class, ICollection<TValue>
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self), "Dictionary must not be null");
            }

            var success = false;

            if (self.TryGetValue(key, out var collection))
            {
                success = collection.Remove(value);

                if (collection.Count <= 0)
                {
                    self.Remove(key);
                }
            }

            return success;
        }
    }
}
