using Kernel;
using Microsoft.EntityFrameworkCore;
using Music.DbModels;

namespace Music
{
    public sealed class MusicDbContext : DbContext
    {
        public DbSet<YoutubeVideo> YouTubeVideos { get; set; }

        public DbSet<YouTubeChannel> YouTubeChannels { get; set; }

        public DbSet<TrackUserProps> TrackUserProps { get; set; }

        public DbSet<Track> Tracks { get; set; }

        public DbSet<TrackUserPropsTag> TrackUserPropsTags { get; set; }

        public DbSet<User> Users { get; set; }

        public MusicDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options
                .UseSqlServer("Data Source=localhost;Initial Catalog=Music;Integrated Security=True")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                ;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureEntities(typeof(YoutubeVideo).Assembly);
        }
    }
}