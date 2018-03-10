using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace programmersdigest.Util {
    public class PropertiesCache {
        private ConcurrentDictionary<Type, PropertyInfo[]> _propertiesCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public PropertyInfo[] GetPropertiesOf<TViewModel>() {
            return GetPropertiesOf(typeof(TViewModel));
        }

        public PropertyInfo[] GetPropertiesOf(Type type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type), "Type must not be null");
            }

            if (!_propertiesCache.TryGetValue(type, out var properties)) {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                _propertiesCache[type] = properties;
            }

            return properties;
        }
    }
}
