﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Music.DbModels
{
    public class TrackUserProps : IDatabaseEntity
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }

        public long TrackId { get; set; }

        public Track Track { get; set; }

        public string YoutubeVideoId { get; set; }

        public YoutubeVideo YoutubeVideo { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<TrackUserPropsTag> TrackTags { get; set; }

        [Required] public DateTime InsertedAt { get; set; }
    }

    public class TrackUserPropsTag : IDatabaseEntity
    {
        [Required]
        public long TrackUserPropsId { get; set; }

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
