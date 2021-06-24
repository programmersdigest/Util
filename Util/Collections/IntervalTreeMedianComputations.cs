using System;

namespace programmersdigest.Util.Collections
{
    public delegate T IntervalTreeMedianComputation<T>(T min, T max);

    public static class IntervalTreeMedianComputations
    {
        public static short ShortMedian(short min, short max) => (short)((min + max) / 2);
        
        public static int IntMedian(int min, int max) => (min + max) / 2;
        
        public static long LongMedian(long min, long max) => (min + max) / 2L;
        
        public static float FloatMedian(float min, float max) => (min + max) / 2f;
        
        public static double DoubleMedian(double min, double max) => (min + max) / 2d;
        
        public static decimal DecimalMedian(decimal min, decimal max) => (min + max) / 2m;

        public static DateTime DateTimeMedian(DateTime min, DateTime max) => min + (max - min);
    }
}
