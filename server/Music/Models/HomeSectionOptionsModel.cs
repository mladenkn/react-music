namespace Music.Models
{
    public class HomeSectionOptionsModel
    {
        public class TracklistOptions
        {
            public class Query_
            {
                public string Type { get; set; }

                public MusicDbTrackQueryParamsModel MusicDbQuery { get; set; }

                public string YouTubeQuery { get; set; }
            }

            public Query_ Query { get; set; }

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
                Query = new TracklistOptions.Query_
                {
                    Type = "MusicDbQuery",
                    MusicDbQuery = new MusicDbTrackQueryParamsModel
                    {
                        SupportedYouTubeChannelsIds = new string[0],
                        MustHaveEveryTag = new string[0],
                        MustHaveAnyTag = new string[0],
                        MusntHaveEveryTag = new string[0],
                        Randomize = true,
                        Skip = 0,
                        Take = 30,
                        YearRange = new MusicDbTrackQueryParamsModel.YearRange_(),
                        RelatedTracks = new long[0]
                    }
                }
            }
        };
    }
}
