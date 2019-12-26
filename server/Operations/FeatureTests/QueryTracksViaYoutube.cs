using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Music;
using Music.DataAccess;
using Music.DataAccess.Models;
using Xunit;

namespace Executables.FeatureTests
{
    public class QueryTracksViaYoutube
    {
        [Fact]
        public async Task Run()
        {
            var builder = new WebHostBuilder().UseStartup<Startup>();

            var videosInDb = new[]
            {
                new YoutubeVideo
                {
                }
            };

            var videosFromApiList = new[]
            {
                new Video
                {

                }
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
                db.Database.EnsureCreated();
            });

            var server = new TestServer(builder);
            var client = server.CreateClient();

            //var r = await client.GetAsync("api/tracks/yt?searchQuery=mia");

            //Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        }
    }
}
