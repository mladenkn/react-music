﻿namespace Music.Domain.Shared
{
    public class HomeSectionOptionsModel
    {
        public class TracklistOptions
        {
            public class QueryForm_
            {
                public TracksQuery MusicDbQuery { get; set; }

                public string YoutubeQuery { get; set; }
            }

            public QueryForm_ QueryForm { get; set; }

            public bool AutoRefresh { get; set; }

            public bool AutoPlay { get; set; }
        }

        public TracklistOptions Tracklist { get; set; }

        public bool TracklistShown { get; set; }
    }
}
