using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Models
{
    public class ListWithTotalCount<T>
    {
        public IReadOnlyList<T> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
