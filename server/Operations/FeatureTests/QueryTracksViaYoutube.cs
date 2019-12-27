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
using Music.Domain.Shared;
using Newtonsoft.Json;
using Utilities;
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

            var searchedVideoIds = new[]
            {
                _gen.String(), _gen.String(), _gen.String(),
            };

            var videosInDb = new[]
            {
                _gen.YoutubeVideo(v =>
                {
                    v.Id = searchedVideoIds[0];
                    v.YoutubeChannel = _gen.YoutubeChannel(c => { c.Id = _gen.String(); });
                }),
                _gen.YoutubeVideo(v =>
                {
                    v.Id = _gen.String();
                    v.YoutubeChannel = _gen.YoutubeChannel(c => { c.Id = _gen.String(); });
                }),
                _gen.YoutubeVideo(v =>
                {
                    v.Id = _gen.String();
                    v.YoutubeChannel = _gen.YoutubeChannel(c => { c.Id = _gen.String(); });
                }),
            };

            var videosFromApiList = new []
            {
                _gen.YoutubeVideoModel(v =>
                {
                    v.Id = searchedVideoIds[1];
                    v.ChannelId = _gen.String();
                }),
                _gen.YoutubeVideoModel(v =>
                {
                    v.Id = searchedVideoIds[2];
                    v.ChannelId = _gen.String();
                }),
            };

            var shouldBeVideoIdsInDbAtTheEnd = Enumerable.Union(searchedVideoIds, videosInDb.Select(v => v.Id));

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<MusicDbContext>));
                if (descriptor != null) 
                    services.Remove(descriptor);

                services.AddDbContext<MusicDbContext>(o => o.UseSqlServer(Config.TestDatabaseConnectionString));
                services.AddTransient<SearchYoutubeVideosIds>(_ => async searchQuery => searchedVideoIds);
                services.AddTransient<ListYoutubeVideos>(_ => async (parts, ids) =>
                {
                    Assert.True(CollectionUtils.AreEquivalentNoOrder(ids, videosFromApiList.Select(v => v.Id)));
                    return videosFromApiList;
                });
            });

            using var server = new TestServer(builder);

            using var servicesScope = server.Services.CreateScope();
            var db = servicesScope.ServiceProvider.GetService<MusicDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.AddRange(videosInDb);
            db.SaveChanges();

            using var client = server.CreateClient();

            var serverResponse = await client.GetAsync("api/tracks/yt?searchQuery=mia");
            Assert.Equal(HttpStatusCode.OK, serverResponse.StatusCode);

            var serverResponseJson = await serverResponse.Content.ReadAsStringAsync();
            var serverResponseTracks = JsonConvert.DeserializeObject<TrackModel[]>(serverResponseJson);
            Assert.True(
                CollectionUtils.AreEquivalentNoOrder(searchedVideoIds, serverResponseTracks.Select(t => t.YoutubeVideoId))
            );

            using (var services = new Services(DatabaseType.SqlServer))
            {
                var allVideoIds = services.DbContext.YoutubeVideos.Select(v => v.Id);
                Assert.True(CollectionUtils.AreEquivalentNoOrder(shouldBeVideoIdsInDbAtTheEnd, allVideoIds));
                services.DbContext.Database.EnsureDeleted();
            }
        }
    }
}
