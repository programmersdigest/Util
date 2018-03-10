using System;
using System.Collections.Generic;

namespace programmersdigest.Util {
    public static class LinqExtensions {
        public static IEnumerable<U> Scan<T, U>(this IEnumerable<T> collection, Func<U, T, U> aggregateFunction, U state = default(U)) {
            foreach (var item in collection) {
                state = aggregateFunction(state, item);
                yield return state;
            }
        }
    }
}
