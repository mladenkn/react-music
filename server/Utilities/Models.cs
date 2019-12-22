using System.Collections.Generic;

namespace Utilities
{
    public class ArrayWithTotalCount<T>
    {
        public T[] Data { get; }

        public int TotalCount { get; }

        public ArrayWithTotalCount(T[] data, int totalCount)
        {
            Data = data;
            TotalCount = totalCount;
        }
    }

    public class Range<T>
    {
        public T LowerBound { get; set; }

        public T UpperBound { get; set; }
    }
}
