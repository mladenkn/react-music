using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Models;

namespace Music.DataAccess
{
    public class MusicDbContext : DbContext
    {
        public MusicDbContext(DbContextOptions<MusicDbContext> o) : base(o)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YoutubeVideo>()
                .HasOne(v => v.TrackUserProps)
                .WithOne(t => t.YoutubeVideo)
                .HasForeignKey<TrackUserProps>(t => t.YoutubeVideoId)
                ;

            modelBuilder.Entity<TrackUserPropsTag>()
                .HasKey(trackUserPropsTags => trackUserPropsTags.Value)
                ;

            modelBuilder.Entity<User>()
                ;
        }
    }
}
