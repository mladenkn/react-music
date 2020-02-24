using Music.Domain.Shared;

namespace Music.DataAccess.Models
{
    public class HomeSectionOptions
    {
        public class TracklistOptions
        {
            public class QueryForm_
            {
                public TracksQuery MusicDbParams { get; set; }

                public string SearchQuery { get; set; }
            }

            public QueryForm_ QueryForm { get; set; }

            public bool AutoRefresh { get; set; }

            public bool AutoPlay { get; set; }
        }

        public TracklistOptions Tracklist { get; set; }

        public bool TracklistShown { get; set; }
    }
}
