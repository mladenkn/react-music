using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Models;

namespace Music.DataAccess
{
    public class MusicDbContext : DbContext
    {
        public DbSet<YoutubeVideo> YoutubeVideos { get; set; }

        public DbSet<TrackUserProps> TrackUserProps { get; set; }
        
        public DbSet<YoutubeChannel> YoutubeChannels { get; set; }

        public MusicDbContext(DbContextOptions<MusicDbContext> o) : base(o)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrackUserProps>()
                .HasMany(t => t.TrackUserPropsTags)
                .WithOne()
                ;
        }
    }
}
