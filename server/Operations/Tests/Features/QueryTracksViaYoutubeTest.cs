using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Executables.Helpers;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.DependencyInjection;
using Music.Domain.QueryTracksViaYoutube;
using Music.Domain.Shared;
using Newtonsoft.Json;
using Utilities;
using Xunit;

namespace Executables.Tests.Features
{
    public class QueryTracksViaYoutubeTest
    {
        private readonly DataGenerator _gen = new DataGenerator();

        [Fact]
        public async Task General()
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
                _gen.Video(v =>
                {
                    v.Id = searchedVideoIds[1];
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
                _gen.Video(v =>
                {
                    v.Id = searchedVideoIds[2];
                    v.ContentDetails = new VideoContentDetails
                    {
                        Duration = "PT3H2M31S"
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
            };

            var shouldBeVideoIdsInDbAtTheEnd = Enumerable.Union(searchedVideoIds, videosInDb.Select(v => v.Id));

            await ServerTest.Run(options =>
            {
                options
                    .ConfigureServices(services =>
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
        [Fact]
        public async Task DataShouldBeRegularlyPersistedAndRead()
        {
            var searchedVideoIds = new[]
            {
                _gen.String(), _gen.String(),
            };

            var videosFromApiList = new[]
            {
                _gen.Video(v =>
                {
                    v.Id = searchedVideoIds[0];
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
                _gen.Video(v =>
                {
                    v.Id = searchedVideoIds[1];
                    v.ContentDetails = new VideoContentDetails
                    {
                        Duration = "PT3H2M31S"
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
            };

            await ServerTest.Run(options =>
            {
                options
                    .ConfigureServices(services =>
                    {
                        services.AddTransient<SearchYoutubeVideosIds>(_ => async searchQuery => searchedVideoIds);
                        services.AddTransient<ListYoutubeVideos>(_ => async (parts, ids) =>
                        {
                            Assert.True(CollectionUtils.AreEquivalentNoOrder(ids, videosFromApiList.Select(v => v.Id)));
                            return videosFromApiList;
                        });
                    })
                    .Act(httpClient => httpClient.GetAsync("api/tracks/yt?searchQuery=mia"))
                    .Assert(async (serverResponse, db) =>
                    {
                        Assert.Equal(HttpStatusCode.OK, serverResponse.StatusCode);

                        var allVideoIds = db.YoutubeVideos.Select(v => v.Id);
                    });
            });
        }
    }
}