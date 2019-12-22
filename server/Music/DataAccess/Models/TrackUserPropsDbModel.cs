using System.Collections.Generic;

namespace Music.DataAccess.Models
{
    public class TrackUserPropsDbModel
    {
        public int UserId { get; set; }

        public long TrackYtId { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }
    }
}
