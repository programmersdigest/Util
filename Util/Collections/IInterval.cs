using System;

namespace programmersdigest.Util.Collections
{
    public interface IInterval<T> where T : IComparable
    {
        T Start { get; }
        T End { get; }
    }
}
