using System.Collections.Generic;

namespace Music.DataAccess.Models
{
    public class TrackUserProps
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string YoutubeVideoId { get; set; }

        public YoutubeVideo YoutubeVideo { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<TrackUserPropsTag> TrackUserPropsTags { get; set; }
    }

    public class TrackUserPropsTag
    {
        public string TrackUserPropsId { get; set; }

        public string Value { get; set; }
    }
}
