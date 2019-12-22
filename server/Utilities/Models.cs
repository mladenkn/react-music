using System.Collections.Generic;

namespace Utilities
{
    public class ListWithTotalCount<T>
    {
        public IReadOnlyList<T> Data { get; set; }

        public int TotalCount { get; set; }
    }
}
