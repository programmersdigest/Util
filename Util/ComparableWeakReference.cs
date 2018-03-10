using System;

namespace programmersdigest.Util {
    public class ComparableWeakReference<T> where T : class {
        private WeakReference<T> _internalReference;
        private int _hashCode;

        public ComparableWeakReference(T target) {
            _internalReference = new WeakReference<T>(target);
            _hashCode = target?.GetHashCode() ?? 0;
        }

        public bool TryGetTarget(out T target) {
            return _internalReference.TryGetTarget(out target);
        }

        public override int GetHashCode() {
            return _hashCode;
        }

        public override bool Equals(object obj) {
            return obj is ComparableWeakReference<T> &&
                   _hashCode == obj.GetHashCode();
        }
    }
}
