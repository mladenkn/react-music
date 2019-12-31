using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Music.DataAccess.Models
{
    public class TrackUserProps
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

        public IReadOnlyCollection<TrackUserPropsTag> TrackTags { get; set; }
    }

    public class TrackUserPropsTag
    {
        [Required]
        public int TrackUserPropsId { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
