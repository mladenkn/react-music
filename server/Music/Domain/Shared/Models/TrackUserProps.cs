using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Music.Domain.Shared.Models
{
    [DatabaseEntity]
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

        [Required] public DateTime InsertedAt { get; set; }
    }

    [DatabaseEntity]
    public class TrackUserPropsTag
    {
        [Required]
        public int TrackUserPropsId { get; set; }

        [Required]
        public string Value { get; set; }

        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrackUserPropsTag>()
                .HasKey(trackUserPropsTags => new { trackUserPropsTags.TrackUserPropsId, trackUserPropsTags.Value })
                ;
        }
    }
}
