using System.Linq;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Models;
using Utilities;

namespace Music.DataAccess
{
    public sealed class MusicDbContext : DbContext
    {
        public DbSet<YoutubeVideo> YoutubeVideos { get; set; }

        public DbSet<Track> Tracks { get; set; }

        public DbSet<TrackTag> TrackTags { get; set; }

        public MusicDbContext(DbContextOptions<MusicDbContext> o) : base(o)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YoutubeVideo>()
                .HasOne(v => v.Track)
                .WithOne(t => t.YoutubeVideo)
                .HasForeignKey<Track>(t => t.YoutubeVideoId)
                ;

            modelBuilder.Entity<TrackTag>()
                .HasKey(trackUserPropsTags => trackUserPropsTags.Value)
                ;

            modelBuilder.Entity<User>()
                ;

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.IsAnyOf(typeof(YoutubeVideoStatistics), typeof(YoutubeVideoTopicDetails)))
                {
                }
                else
                    entityType.SetTableName(entityType.DisplayName() + "s");
            }
        }
    }
}