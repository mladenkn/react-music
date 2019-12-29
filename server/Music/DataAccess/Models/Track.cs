using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Music.DataAccess.Models
{
    public class Track
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        public string YoutubeVideoId { get; set; }

        public YoutubeVideo YoutubeVideo { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<TrackTag> TrackTags { get; set; }
    }

    public class TrackTag
    {
        [Required]
        public int TrackId { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
