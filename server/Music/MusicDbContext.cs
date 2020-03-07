﻿using Kernel;
using Microsoft.EntityFrameworkCore;
using Music.Admin.Models;
using Music.App.DbModels;
using Utilities;

namespace Music
{
    public sealed class MusicDbContext : DbContext
    {
        public DbSet<YoutubeVideo> YoutubeVideos { get; set; }

        public DbSet<YouTubeChannel> YouTubeChannels { get; set; }

        public DbSet<TrackUserProps> TrackUserProps { get; set; }

        public DbSet<Track> Tracks { get; set; }

        public DbSet<TrackUserPropsTag> TrackUserPropsTags { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserAdminData> UserAdminData { get; set; }

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
        }
    }
}