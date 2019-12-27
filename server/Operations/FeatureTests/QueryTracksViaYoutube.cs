using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

            var videosFromApiList = new[]
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

            await ServerTest.Run(options =>
            {
                options.ConfigureServices(services =>
                    {
                        services.AddTransient<SearchYoutubeVideosIds>(_ => async searchQuery => searchedVideoIds);
                        services.AddTransient<ListYoutubeVideos>(_ => async (parts, ids) =>
                        {
                            Assert.True(CollectionUtils.AreEquivalentNoOrder(ids, videosFromApiList.Select(v => v.Id)));
                            return videosFromApiList;
                        });
                    })
                    .PrepareDatabase(db =>
                    {
                        db.AddRange(videosInDb);
                        db.SaveChanges();
                    })
                    .Act(httpClient => httpClient.GetAsync("api/tracks/yt?searchQuery=mia"))
                    .Assert(async (serverResponse, db) =>
                    {
                        Assert.Equal(HttpStatusCode.OK, serverResponse.StatusCode);

                        var responseContent =
                            JsonConvert.DeserializeObject<TrackModel[]>(
                                await serverResponse.Content.ReadAsStringAsync());

                        Assert.True(
                            CollectionUtils.AreEquivalentNoOrder(searchedVideoIds,
                                responseContent.Select(t => t.YoutubeVideoId))
                        );

                        var allVideoIds = db.YoutubeVideos.Select(v => v.Id);
                        Assert.True(CollectionUtils.AreEquivalentNoOrder(shouldBeVideoIdsInDbAtTheEnd, allVideoIds));
                    });
            });
        }
    }

    public class ServerTestOptions
    {
        public Action<IServiceCollection> ConfigureServices { get; set; }
        public Action<MusicDbContext> PrepareDatabase { get; set; }
        public Func<HttpClient, Task<HttpResponseMessage>> Act { get; set; }
        public Func<HttpResponseMessage, MusicDbContext, Task> Assert { get; set; }
    }

    public class ServerTestOptionsBuilder
    {
        private readonly ServerTestOptions _options = new ServerTestOptions();

        public ServerTestOptionsBuilder ConfigureServices(Action<IServiceCollection> configureServicesActual)
        {
            _options.ConfigureServices = configureServicesActual;
            return this;
        }

        public ServerTestOptionsBuilder PrepareDatabase(Action<MusicDbContext> prepareDatabaseActual)
        {
            _options.PrepareDatabase = prepareDatabaseActual;
            return this;
        }

        public ServerTestOptionsBuilder Act(Func<HttpClient, Task<HttpResponseMessage>> actActual)
        {
            _options.Act = actActual;
            return this;
        }

        public ServerTestOptionsBuilder Assert(Func<HttpResponseMessage, MusicDbContext, Task> assertActual)
        {
            _options.Assert = assertActual;
            return this;
        }

        public ServerTestOptions Build() => _options;
    }

    public class ServerTest
    {
        public static async Task Run(Action<ServerTestOptionsBuilder> addOptions)
        {
            var optionsBuilder = new ServerTestOptionsBuilder();
            addOptions(optionsBuilder);
            var options = optionsBuilder.Build();
            await Run(options);
        }

        public static async Task Run(ServerTestOptions options)
        {
            var builder = new WebHostBuilder().UseStartup<Startup>();
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MusicDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
                services.AddDbContext<MusicDbContext>(o => o.UseSqlServer(Config.TestDatabaseConnectionString));
                options.ConfigureServices(services);
            });

            var server = new TestServer(builder);
            var client = server.CreateClient();
            var servicesScope = server.Services.CreateScope();
            var serviceProvider = servicesScope.ServiceProvider;

            var db = serviceProvider.GetService<MusicDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            options.PrepareDatabase(db);
            var serverResponse = await options.Act(client);
            await options.Assert(serverResponse, db);

            db.Database.EnsureDeleted();
            db.Dispose();
            server.Dispose();
            client.Dispose();
            servicesScope.Dispose();
        }
    }
}