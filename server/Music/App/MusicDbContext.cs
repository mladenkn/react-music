using Kernel;
using Microsoft.EntityFrameworkCore;
using Music.App.Models;
using Utilities;

namespace Music.App
{
    public sealed class MusicDbContext : DbContext
    {
        public DbSet<YoutubeVideo> YoutubeVideos { get; set; }

        public DbSet<YoutubeChannel> YouTubeChannels { get; set; }

        public DbSet<TrackUserProps> TrackUserProps { get; set; }

        public DbSet<TrackUserPropsTag> TrackTags { get; set; }

        public DbSet<User> Users { get; set; }

        public MusicDbContext(DbContextOptions options) : base(options)
        {
        }

        public static void Configure(DbContextOptionsBuilder options)
        {
            options
                .UseSqlServer("Data Source=localhost;Initial Catalog=Music;Integrated Security=True")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                ;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureEntities(typeof(YoutubeVideo).Assembly);

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