using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Executables.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Music;
using Music.DataAccess;
using Music.DataAccess.Models;
using Utilities;
using Guid = System.Guid;

namespace Executables.Tests
{
    public class E2eTest
    {
        private readonly DataGenerator _gen = new DataGenerator();

        public async Task Run()
        {
            var users = CollectionUtils.Repeat(() => _gen.User(), 4);
            var activeUser = users[0];

            var trackUserProps = new[]
            {
                _gen.Track(t =>
                {
                    t.User = activeUser;
                    t.YoutubeVideo = GenerateYoutubeVideo();
                }),
                _gen.Track(t =>
                {
                    t.User = activeUser;
                    t.YoutubeVideo = GenerateYoutubeVideo();
                    t.TrackTags = CollectionUtils.Repeat(() => _gen.TrackTag(), 3);
                }),
                _gen.Track(t =>
                {
                    t.User = activeUser;
                    t.YoutubeVideo = GenerateYoutubeVideo();
                }),
                _gen.Track(t =>
                {
                    t.User = users[1];
                    t.YoutubeVideo = GenerateYoutubeVideo();
                }),
                _gen.Track(t =>
                {
                    t.User = users[1];
                    t.YoutubeVideo = GenerateYoutubeVideo();
                }),
                _gen.Track(t =>
                {
                    t.User = users[1];
                    t.YoutubeVideo = GenerateYoutubeVideo();
                }),
                _gen.Track(t =>
                {
                    t.User = users[2];
                    t.YoutubeVideo = GenerateYoutubeVideo();
                }),
            };

            var dbName = Guid.NewGuid().ToString();
            
            using (var db = Utils.UseDatabase(dbName))
            {
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
                db.AddRange(trackUserProps);
                await db.SaveChangesAsync();
            }
            
            var builder = new WebHostBuilder().UseStartup<Startup>();

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MusicDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
                services.AddDbContext<MusicDbContext>(o => o.UseSqlServer(Config.GetTestDatabaseConnectionString(dbName)));
            });

            using var server = new TestServer(builder);
            using var client = server.CreateClient();

            
            // obični query, assert
            // query preko yt i dodaje 2 trake, uzme traku iz odgovora od servera, nešto postavi i posta na server
            // obični query, assert
            // briše neku traku
            // obični query, assert
        }


        private YoutubeVideo GenerateYoutubeVideo()
        {
            return _gen.YoutubeVideo(v =>
            {
                v.Id = _gen.String();
                v.YoutubeChannelId = _gen.String();
                v.YoutubeChannel = _gen.YoutubeChannel(c => { c.Id = v.YoutubeChannelId; });
                v.Thumbnails = new[]
                {
                    _gen.YoutubeVideoThumbnail(t => { t.Name = "Default__"; }),
                    _gen.YoutubeVideoThumbnail(),
                };
            });
        }
    }
}
