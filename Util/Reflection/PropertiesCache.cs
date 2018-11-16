using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace programmersdigest.Util.Reflection
{
    /// <summary>
    /// Provides a cache for reflection based property access.
    /// By default, type.GetProperties() is not cached. This may lead to performance issues in
    /// cases where properties have to be retrieved many times (e.g. in loops over large collections).
    /// This class provides a convenient way to cache properties for the given types for faster access
    /// in these special cases.
    /// </summary>
    public class PropertiesCache
    {
        private ConcurrentDictionary<Type, PropertyInfo[]> _propertiesCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Gets the properties of the given <typeparamref name="T"/> using the cache if possible.
        /// On first access for a specific type, the cache has to be filled. Subsequent access is
        /// cached and can thus be done faster.
        /// </summary>
        /// <typeparam name="T">The type of which to get the properties.</typeparam>
        /// <returns>An array of all properties of <typeparamref name="T"/>.</returns>
        public PropertyInfo[] GetPropertiesOf<T>()
        {
            return GetPropertiesOf(typeof(T));
        }

        /// <summary>
        /// Gets the properties of the given <paramref name="type"/> using the cache if possible.
        /// On first access for a specific type, the cache has to be filled. Subsequent access is
        /// cached and can thus be done faster.
        /// </summary>
        /// <param name="type">The type of which to get the properties.</param>
        /// <returns>An array of all properties of <paramref name="type"/>.</returns>
        public PropertyInfo[] GetPropertiesOf(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type), "Type must not be null");
            }

            if (!_propertiesCache.TryGetValue(type, out var properties))
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                _propertiesCache[type] = properties;
            }

            return properties;
        }
    }
}
