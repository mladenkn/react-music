﻿namespace Music.Domain.Shared
{
    public class HomeSectionOptionsModel
    {
        public class TracklistOptions
        {
            public class QueryForm_
            {
                public string DataSource { get; set; }

                public TracksQuery MusicDbQuery { get; set; }

                public string YouTubeQuery { get; set; }
            }

            public QueryForm_ QueryForm { get; set; }

            public bool AutoRefresh { get; set; }

            public bool AutoPlay { get; set; }
        }

        public TracklistOptions Tracklist { get; set; }

        public bool TracklistShown { get; set; }

        public static HomeSectionOptionsModel CreateInitial() => new HomeSectionOptionsModel
        {
            TracklistShown = true,
            Tracklist = new TracklistOptions
            {
                AutoPlay = true,
                AutoRefresh = true,
                QueryForm = new TracklistOptions.QueryForm_
                {
                    DataSource = "MusicDb",
                    MusicDbQuery = new TracksQuery
                    {
                        MustHaveEveryTag = new string[0],
                        MustHaveAnyTag = new string[0],
                        Randomize = true,
                        Skip = 0,
                        Take = 30,
                        YearRange = new TracksQuery.YearRange_(),
                    }
                }
            }
        };
    }
}