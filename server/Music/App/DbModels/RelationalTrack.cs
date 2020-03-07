using System.Collections.Generic;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Music.App.DbModels
{
    public class RelationalTrack : IDatabaseEntity
    {
        public long Id { get; set; }

        public IReadOnlyCollection<TrackUserProps> TrackUserProps { get; set; }

        public IReadOnlyCollection<YoutubeVideo> YoutubeVideos { get; set; }

        public bool IsDuplicate { get; set; }

        public static void Configure(ModelBuilder modelBuilder) =>
            modelBuilder.Entity<RelationalTrack>(c =>
            {
                c.ToTable("Tracks");
            });
    }
}
