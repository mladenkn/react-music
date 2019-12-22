using System.Collections.Generic;

namespace Utilities
{
    public class ListWithTotalCount<T>
    {
        public IReadOnlyList<T> Data { get; }

        public int TotalCount { get; }

        public ListWithTotalCount(IReadOnlyList<T> data, int totalCount)
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
