﻿using System.Collections.Generic;

namespace Music.Models
{
    public class MusicDbTrackQueryParamsModel
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public string TitleContains { get; set; }

        public IEnumerable<string> SupportedYouTubeChannelsIds { get; set; }

        public IReadOnlyCollection<string> MustHaveEveryTag { get; set; }

        public IReadOnlyCollection<string> MustHaveAnyTag { get; set; }

        public IReadOnlyCollection<string> MusntHaveEveryTag { get; set; }

        public YearRange_ YearRange { get; set; }

        public bool Randomize { get; set; }

        public long[] RelatedTracks { get; set; }

        public class YearRange_
        {
            public int? LowerBound { get; set; }

            public int? UpperBound { get; set; }
        }
    }
}
