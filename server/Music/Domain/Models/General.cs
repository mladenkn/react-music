using System.Collections.Generic;

namespace Music.Domain.Models
{
    public class ListWithTotalCount<T>
    {
        public IReadOnlyList<T> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
