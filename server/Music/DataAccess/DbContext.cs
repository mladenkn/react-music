using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Models;

namespace Music.DataAccess
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<YoutubeVideoDbModel> YoutubeVideos { get; set; }

        public DbSet<TrackUserPropsDbModel> TrackUserProps { get; set; }
    }
}
