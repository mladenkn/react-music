using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Models;
using Utilities;

namespace Music.DataAccess
{
    public sealed class MusicDbContext : DbContext
    {
        public DbSet<YoutubeVideo> YoutubeVideos { get; set; }

        public DbSet<TrackUserProps> TrackUserProps { get; set; }

        public DbSet<TrackUserPropsTag> TrackTags { get; set; }

        public static void Configure(DbContextOptionsBuilder options)
        {
            options
                .UseSqlServer("Data Source=localhost;Initial Catalog=Music;Integrated Security=True")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                ;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YoutubeVideo>()
                .HasMany(v => v.TrackUserProps)
                .WithOne(t => t.YoutubeVideo)
                ;

            modelBuilder.Entity<TrackUserPropsTag>()
                .HasKey(trackUserPropsTags => new { trackUserPropsTags.TrackUserPropsId, trackUserPropsTags.Value })
                ;

            modelBuilder.Entity<User>()
                ;

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.IsAnyOf(typeof(YoutubeVideoStatistics), typeof(YoutubeVideoTopicDetails), typeof(TrackUserProps)))
                    ;
                else
                    entityType.SetTableName(entityType.DisplayName() + "s");
            }
        }
    }
}