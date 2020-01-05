using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Executables.Helpers;
using FluentAssertions;
using Google.Apis.YouTube.v3.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Music.DataAccess.Models;
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
        private readonly ServerTestOptionsBuilder _testOptions = new ServerTestOptionsBuilder();

        public QueryTracksViaYoutubeTest()
        {
            _testOptions.ConfigureServices(services =>
            {
                var curUserContextMock = new Mock<ICurrentUserContext>();
                curUserContextMock.Setup(c => c.Id).Returns(() => 1);
                services.AddTransient<ICurrentUserContext>(sp => curUserContextMock.Object);
            });
        }

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

            _testOptions
                .ConfigureServices(services =>
                {
                    services.AddTransient<SearchYoutubeVideosIds>(_ => async searchQuery => searchedVideoIds);
                    services.AddTransient<ListYoutubeVideos>(_ => async (parts, ids) =>
                    {
                        videosFromApiList.Select(v => v.Id).Should().BeEquivalentTo(ids);
                        return videosFromApiList;
                    });
                })
                .PrepareDatabase(async db =>
                {
                    db.AddRange(videosInDb);
                    await db.SaveChangesAsync();
                })
                .Act(httpClient => httpClient.GetAsync("api/tracks/yt?searchQuery=mia"))
                .Assert(async (serverResponse, db) =>
                {
                    Assert.Equal(HttpStatusCode.OK, serverResponse.StatusCode);

                    var responseContent =
                        JsonConvert.DeserializeObject<TrackModel[]>(
                            await serverResponse.Content.ReadAsStringAsync());

                    responseContent.Select(t => t.YoutubeVideoId).Should().BeEquivalentTo(searchedVideoIds);

                    var allVideoIds = await db.YoutubeVideos.Select(v => v.Id).ToArrayAsync();
                    allVideoIds.Should().BeEquivalentTo(shouldBeVideoIdsInDbAtTheEnd);
                });

            await ServerTest.Run(_testOptions);
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

            _testOptions
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
                    var allVideos = await db.YoutubeVideos
                        .Include(v => v.YoutubeChannel)
                        .Include(v => v.Statistics)
                        .Include(v => v.Tags)
                        .Include(v => v.Thumbnails)
                        .Include(v => v.TopicDetails)
                            .ThenInclude(td => td.RelevantTopicIds)
                        .Include(v => v.TopicDetails)
                            .ThenInclude(td => td.TopicIds)
                        .ToArrayAsync();

                    allVideos.Should().BeEquivalentTo(
                        new[]
                        {
                            new YoutubeVideo
                            {
                                Id = videosFromApiList[0].Id,
                                Title = videosFromApiList[0].Snippet.Title,
                                Description = videosFromApiList[0].Snippet.Description,
                                YoutubeChannel = new YoutubeChannel
                                {
                                    Id = videosFromApiList[0].Snippet.ChannelId,
                                    Title = videosFromApiList[0].Snippet.ChannelTitle,
                                },
                                ThumbnailsEtag = videosFromApiList[0].Snippet.Thumbnails.ETag,
                                YoutubeCategoryId = videosFromApiList[0].Snippet.CategoryId,
                                Duration = XmlConvert.ToTimeSpan(videosFromApiList[0].ContentDetails.Duration),
                                PublishedAt = videosFromApiList[0].Snippet.PublishedAt,
                                YoutubeChannelId = videosFromApiList[0].Snippet.ChannelId,
                            },
                            new YoutubeVideo
                            {
                                Id = videosFromApiList[1].Id,
                                Title = videosFromApiList[1].Snippet.Title,
                                Description = videosFromApiList[1].Snippet.Description,
                                YoutubeChannel = new YoutubeChannel
                                {
                                    Id = videosFromApiList[1].Snippet.ChannelId,
                                    Title = videosFromApiList[1].Snippet.ChannelTitle,
                                },
                                ThumbnailsEtag = videosFromApiList[1].Snippet.Thumbnails.ETag,
                                YoutubeCategoryId = videosFromApiList[1].Snippet.CategoryId,
                                Duration = XmlConvert.ToTimeSpan(videosFromApiList[1].ContentDetails.Duration),
                                PublishedAt = videosFromApiList[1].Snippet.PublishedAt,
                                YoutubeChannelId = videosFromApiList[1].Snippet.ChannelId,
                            }
                        },
                        o => o
                            .Excluding(v => v.Thumbnails)
                            .Excluding(v => v.Tags)
                            .Excluding(v => v.Statistics)
                            .Excluding(v => v.TopicDetails)
                    );

                    allVideos.Should().SatisfyRespectively(
                        item =>
                        {
                            item.Thumbnails.Should().Contain(t => t.Name == "Default__");
                            item.Tags.Select(t => t.Value).Should().BeEquivalentTo(
                                videosFromApiList[0].Snippet.Tags.Select(t => t)
                            );
                            item.Tags.All(t => t.YoutubeVideoId == videosFromApiList[0].Id).Should().BeTrue();
                            item.Statistics.Should().BeEquivalentTo(videosFromApiList[0].Statistics);
                        },
                        item =>
                        {
                            item.Thumbnails.Should().Contain(t => t.Name == "Default__");
                            item.Tags.Select(t => t.Value).Should().BeEquivalentTo(
                                videosFromApiList[1].Snippet.Tags.Select(t => t)
                            );
                            item.Tags.All(t => t.YoutubeVideoId == videosFromApiList[1].Id).Should().BeTrue();
                            item.Statistics.Should().BeEquivalentTo(videosFromApiList[1].Statistics);
                        }
                    );
                });

            await ServerTest.Run(_testOptions);
        }
    }
}