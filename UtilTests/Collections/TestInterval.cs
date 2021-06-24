using programmersdigest.Util.Collections;

namespace programmersdigest.UtilTests.Collections
{
    internal sealed class TestInterval : IInterval<int>
    {
        public int Start { get; }
        public int End { get; }

        public TestInterval(int start, int end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return $"[{Start},{End}]";
        }

        public static implicit operator TestInterval((int Start, int End) values) => new TestInterval(values.Start, values.End);
    }
}
