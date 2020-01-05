using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Executables.Helpers;
using FluentAssertions;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Music;
using Music.DataAccess;
using Music.DataAccess.Models;
using Music.Domain.QueryTracksViaYoutube;
using Music.Domain.Shared;
using Utilities;
using Xunit;

namespace Executables.Tests
{
    public class E2eTest
    {
        private readonly DataGenerator _gen = new DataGenerator();

        //[Fact]
        public async Task Run()
        {
            var users = CollectionUtils.Repeat(() => _gen.User(), 4);
            var activeUser = users[0];

            var tracksInDb = new[]
            {
                _gen.Track(t =>
                {
                    t.User = activeUser;
                    t.YoutubeVideo = GenerateYoutubeVideo();
                    t.TrackTags = new TrackUserPropsTag[0];
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
                    t.TrackTags = CollectionUtils.Repeat(() => _gen.TrackTag(), 2);
                }),
                _gen.Track(t =>
                {
                    t.User = users[1];
                    t.YoutubeVideo = GenerateYoutubeVideo();
                    t.TrackTags = new TrackUserPropsTag[0];
                }),
                _gen.Track(t =>
                {
                    t.User = users[1];
                    t.YoutubeVideo = GenerateYoutubeVideo();
                    t.TrackTags = new TrackUserPropsTag[0];
                }),
                _gen.Track(t =>
                {
                    t.User = users[1];
                    t.YoutubeVideo = GenerateYoutubeVideo();
                    t.TrackTags = new TrackUserPropsTag[0];
                }),
                _gen.Track(t =>
                {
                    t.User = users[2];
                    t.YoutubeVideo = GenerateYoutubeVideo();
                    t.TrackTags = new TrackUserPropsTag[0];
                }),
            };

            var vidsFromYtList = CollectionUtils.Repeat(() =>
                _gen.Video(v =>
                {
                    v.Id = _gen.String();
                    v.ContentDetails = new VideoContentDetails
                    {
                        Duration = "PT3M20S"
                    };
                    v.Statistics = _gen.VideoStatistics();
                    v.TopicDetails = _gen.VideoTopicDetails();
                    v.Snippet = _gen.VideoSnippet(s =>
                    {
                        s.Thumbnails = new ThumbnailDetails
                        {
                            Default__ = _gen.Thumbnail(),
                            ETag = _gen.String(),
                            Medium = _gen.Thumbnail(),
                        };
                    });
                }), 
                5
            );

            var videoIdsFromYoutubeSearch = vidsFromYtList.Select(v => v.Id);

            TrackModel TrackModelMatcherOfIndex(int index)
            {
                var trackUserProps = tracksInDb[index];
                return new TrackModel
                {
                    Description = trackUserProps.YoutubeVideo.Description,
                    YoutubeChannelId = trackUserProps.YoutubeVideo.YoutubeChannelId,
                    Title = trackUserProps.YoutubeVideo.Title,
                    YoutubeVideoId = trackUserProps.YoutubeVideoId,
                    Image = trackUserProps.YoutubeVideo.Thumbnails.FirstOrDefault(t => t.Name == "Default__")
                        ?.Url,
                    YoutubeChannelTitle = trackUserProps.YoutubeVideo.YoutubeChannel.Title,
                    Tags = trackUserProps.TrackTags.Select(tt => tt.Value).ToArray(),
                    Year = trackUserProps.Year,
                };
            }

            var dbClient = await TestDatabaseClient.Create();

            await dbClient.UseIt(async db =>
            {
                db.AddRange(tracksInDb);
                await db.SaveChangesAsync();
            });

            var server = Server.Create(
                dbClient.DatabaseName, 
                async _ => videoIdsFromYoutubeSearch, 
                async (_, __) => vidsFromYtList,
                activeUser.Id
            );

            var query1Response_Tracks = await server.DoRequest<ArrayWithTotalCount<TrackModel>>(httpClient => 
                httpClient.GetAsync("api/tracks?skip=0&take=100")
            );
            query1Response_Tracks.Should().BeEquivalentTo(
                new ArrayWithTotalCount<TrackModel>(
                 new []
                 {
                     TrackModelMatcherOfIndex(0),
                     TrackModelMatcherOfIndex(1),
                     TrackModelMatcherOfIndex(2),
                 }, 
                3)
            );

            var ytQueryResponse_Tracks = await server.DoRequest<TrackModel[]>(httpClient =>
                httpClient.GetAsync("/api/tracks/yt?searchQuery=mia")
            );

            var ytQueryResponse_Track1 = ytQueryResponse_Tracks[0];
            ytQueryResponse_Track1.Tags = new List<string>(ytQueryResponse_Track1.Tags) { "dodani tag" };

            // obični query, assert
            // query preko yt i dodaje 1 traku (uzme traku iz odgovora od servera, nešto postavi i posta na server)
            // obični query, assert
            // briše neku traku
            // obični query sa filterima, assert
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

    internal class Server : IDisposable
    {
        private readonly TestServer _testServer;
        private readonly HttpClient _httpClient;

        public Server(TestServer testServer)
        {
            _testServer = testServer;
            _httpClient = testServer.CreateClient();
        }

        public static Server Create(
            string dbName, 
            SearchYoutubeVideosIds searchYoutubeVideosIds, 
            ListYoutubeVideos listYoutubeVideos,
            int currentUserId
        )
        {
            var builder = new WebHostBuilder().UseStartup<Startup>();

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MusicDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
                
                var currentUserContextMock = new Mock<ICurrentUserContext>();
                currentUserContextMock.Setup(c => c.Id).Returns(currentUserId);
                services.AddTransient<ICurrentUserContext>(sp => currentUserContextMock.Object);

                services.AddDbContext<MusicDbContext>(o => o.UseSqlServer(Config.GetTestDatabaseConnectionString(dbName)));
                services.AddTransient<SearchYoutubeVideosIds>(sp => searchYoutubeVideosIds);
                services.AddTransient<ListYoutubeVideos>(sp => listYoutubeVideos);
            });

            return new Server(new TestServer(builder));
        }

        public async Task<TContent> DoRequest<TContent>(Func<HttpClient, Task<HttpResponseMessage>> doRequest)
        {
            var firstReqResponse = await doRequest(_httpClient);
            firstReqResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var contentParsed = await firstReqResponse.Content.ParseAsJson<TContent>();
            return contentParsed;
        }

        public void Dispose()
        {
            _testServer?.Dispose();
            _httpClient?.Dispose();
        }
    }
}
