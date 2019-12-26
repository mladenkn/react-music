using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;
using Kernel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Music;
using Music.DataAccess;
using Music.Domain.QueryTracksViaYoutube;
using Xunit;

namespace Executables.FeatureTests
{
    public class QueryTracksViaYoutube
    {
        private readonly DataGenerator _gen = new DataGenerator();

        [Fact]
        public async Task Run()
        {
            var builder = new WebHostBuilder().UseStartup<Startup>();

            var shouldFetchVideosIds = new[]
            {
                _gen.String(), _gen.String(), _gen.String(),
            };

            var videosInDb = new[]
            {
                _gen.YoutubeVideo(v => { v.Id = shouldFetchVideosIds[0]; }),
                _gen.YoutubeVideo(v => { v.Id = _gen.String(); }),
                _gen.YoutubeVideo(v => { v.Id = _gen.String(); }),
            };

            var videosFromApiList = new List<Video>
            {
                _gen.Video(v => { v.Id = shouldFetchVideosIds[0]; }),
                _gen.Video(v => { v.Id = shouldFetchVideosIds[1]; }),
                _gen.Video(v => { v.Id = shouldFetchVideosIds[2]; }),
            };

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<MusicDbContext>));
                if (descriptor != null) 
                    services.Remove(descriptor);

                services.AddDbContext<MusicDbContext>(o => o.UseSqlServer(Config.TestDatabaseConnectionString));

                var sp = services.BuildServiceProvider();
                using var serviceScope = sp.CreateScope();

                var db = serviceScope.ServiceProvider.GetService<MusicDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.AddRange(videosInDb);
                db.SaveChanges();
                
                services.AddTransient<SearchYoutubeVideosIds>(_ => async searchQuery => shouldFetchVideosIds);

                services.AddTransient<ListYoutubeVideos>(_ => async (parts, ids) =>
                {
                    // assert da su shouldFetchVideosIds[1] i shouldFetchVideosIds[2]
                    return videosFromApiList;
                });
            });

            var server = new TestServer(builder);
            var client = server.CreateClient();

            var r = await client.GetAsync("api/tracks/yt?searchQuery=mia");
            Assert.Equal(HttpStatusCode.OK, r.StatusCode);

            // assert da su shouldFetchVideosIds[1] i shouldFetchVideosIds[2] spremljeni u bazu
        }
    }
}
