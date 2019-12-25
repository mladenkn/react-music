using System.Collections.Generic;

namespace Music.DataAccess.Models
{
    public class Track
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public string YoutubeVideoId { get; set; }

        public YoutubeVideo YoutubeVideo { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<TrackTag> TrackTags { get; set; }
    }

    public class TrackTag
    {
        public int TrackId { get; set; }

        public string Value { get; set; }
    }
}
