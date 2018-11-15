using System;

namespace programmersdigest.Util
{
    /// <summary>
    /// An implementation of a weak reference type which holds a hash code of the target
    /// reference to provide comparability.
    /// </summary>
    /// <typeparam name="T">Type of the reference instance.</typeparam>
    public class ComparableWeakReference<T> where T : class
    {
        private WeakReference<T> _internalReference;
        private int _hashCode;

        /// <summary>
        /// Creates a new <see cref="ComparableWeakReference{T}"/> which references the given <paramref name="target"/>.
        /// </summary>
        /// <param name="target">The target to get a weak reference to.</param>
        public ComparableWeakReference(T target)
        {
            _internalReference = new WeakReference<T>(target);
            _hashCode = target?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Tries to resolve the reference and retrieve the target instance.
        /// </summary>
        /// <param name="target">The referenced instance.</param>
        /// <returns><c>true</c> if the target reference could be retrieved. Otherwise false.</returns>
        public bool TryGetTarget(out T target)
        {
            return _internalReference.TryGetTarget(out target);
        }

        /// <summary>
        /// Gets the stored hash code of the referenced instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _hashCode;
        }

        /// <summary>
        /// Compares this <see cref="WeakReference{T}"/> to the given <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns><c>true</c> if both instances are equal, <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is ComparableWeakReference<T> &&
                   _hashCode == obj.GetHashCode();
        }
    }
}
