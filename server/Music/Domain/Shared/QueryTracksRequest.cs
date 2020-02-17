using System.Collections.Generic;
using Utilities;

namespace Music.Domain.Shared
{
    public class QueryTracksRequest
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public string TitleContains { get; set; }

        public string YoutubeChannelId { get; set; }

        public IReadOnlyCollection<string> MustHaveEveryTag { get; set; }

        public IReadOnlyCollection<string> MustHaveAnyTag { get; set; }

        public Range<int> YearRange { get; set; }
    }
}
