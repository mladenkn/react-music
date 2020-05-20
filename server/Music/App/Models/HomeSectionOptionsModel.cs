﻿namespace Music.App.Models
{
    public class HomeSectionOptionsModel
    {
        public class TracklistOptions
        {
            public class Filter_
            {
                public string DataSource { get; set; }

                public TracksQueryModel MusicDbQuery { get; set; }

                public string YouTubeQuery { get; set; }
            }

            public Filter_ Filter { get; set; }

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
                Filter = new TracklistOptions.Filter_
                {
                    DataSource = "MusicDb",
                    MusicDbQuery = new TracksQueryModel
                    {
                        SupportedYouTubeChannelsIds = new string[0],
                        MustHaveEveryTag = new string[0],
                        MustHaveAnyTag = new string[0],
                        Randomize = true,
                        Skip = 0,
                        Take = 30,
                        YearRange = new TracksQueryModel.YearRange_(),
                    }
                }
            }
        };
    }
}
