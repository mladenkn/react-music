using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Executables.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Music;
using Music.DataAccess;
using Music.DataAccess.Models;
using Music.Domain.Shared;
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

            var dbClient = await TestDatabaseClient.Create();

            await dbClient.UseIt(async db =>
            {
                db.AddRange(trackUserProps);
                await db.SaveChangesAsync();
            });

            var httpClient = SetupAndGetHttpClient(dbClient.DatabaseName);
            var firstReqResponse = await httpClient.GetAsync("/api/tracks/skip=0&take=100");
            var firstReqTracks = await firstReqResponse.Content.ParseAsJson<ArrayWithTotalCount<TrackModel>>();

            // obični query, assert
            // query preko yt i dodaje 2 trake, uzme traku iz odgovora od servera, nešto postavi i posta na server
            // obični query, assert
            // briše neku traku
            // obični query sa filterima, assert
        }

        private HttpClient SetupAndGetHttpClient(string dbName)
        {
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

            return client;
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
