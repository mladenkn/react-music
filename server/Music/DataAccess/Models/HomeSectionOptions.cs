using Music.Domain.Shared;

namespace Music.DataAccess.Models
{
    public class HomeSectionPersistableState
    {
        public class HomeSectionOptions
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

        public HomeSectionOptions Options { get; set; }

        public string SelectedTrackYoutubeId { get; set; }

        public string CurrentTrackYoutubeId { get; set; }
    }
}
