using System;
using System.Linq;
using System.Threading.Tasks;
using Executables.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Models;
using Xunit;

namespace Executables.Tests
{
    public class DbTest
    {
        private readonly DataGenerator _gen = new DataGenerator();

        [Fact]
        public async Task Can_Add_One_YoutubeChannel()
        {
            var channel = _gen.YoutubeChannel(c => { c.Id = _gen.String(); });

            await using var dbClient = await TestDatabaseClient.Create();

            await dbClient.UseIt(
                async db =>
                {
                    db.Add(channel);
                    await db.SaveChangesAsync();
                },
                async db =>
                {
                    var channelFromDb = await db.Set<YoutubeChannel>().SingleAsync();
                    channelFromDb.Id.Should().Be(channel.Id);
                    channelFromDb.Title.Should().Be(channel.Title);
                }
            );
        }
        
        [Fact]
        public async Task Can_Add_YoutubeVideo()
        {
            var video = _gen.YoutubeVideo(
                v =>
                {
                    v.Id = _gen.String();
                    v.YoutubeChannelId = _gen.String();
                    v.YoutubeChannel = _gen.YoutubeChannel(c => { c.Id = v.YoutubeChannelId; });
                }
            );

            await using var dbClient = await TestDatabaseClient.Create();

            await dbClient.UseIt(
                async db =>
                {
                    db.Add(video);
                    await db.SaveChangesAsync();
                },
                async db =>
                {
                    var videoFromDb = await db.Set<YoutubeVideo>().Include(v => v.YoutubeChannel).SingleAsync();
                    videoFromDb.Should().BeEquivalentTo(video);
                }
            );
        }

        [Fact]
        public async Task Can_Add_Full_Track_With_Full_YouTubeVideo()
        {
            var trackUserProps = _gen.Track(t =>
            {
                t.TrackTags = new[]
                {
                    _gen.TrackTag(),
                    _gen.TrackTag(),
                };
                t.User = _gen.User();
                t.YoutubeVideo = _gen.YoutubeVideo(
                    v =>
                    {
                        v.Id = _gen.String();
                        v.Statistics = _gen.YoutubeVideoStatistics();
                        v.Tags = new[]
                        {
                            _gen.YoutubeVideoTag(),
                            _gen.YoutubeVideoTag(),
                        };
                        v.Thumbnails = new[]
                        {
                            _gen.YoutubeVideoThumbnail(),
                            _gen.YoutubeVideoThumbnail(),
                        };
                        v.YoutubeChannelId = _gen.String();
                        v.YoutubeChannel = _gen.YoutubeChannel(
                            c => { c.Id = v.YoutubeChannelId; }
                        );
                        v.TopicDetails = _gen.YoutubeVideoTopicDetails(td =>
                        {
                            foreach (var item in td.TopicCategories)
                                item.YoutubeVideoId = v.Id;
                            foreach (var item in td.TopicIds)
                                item.YoutubeVideoId = v.Id;
                            foreach (var item in td.RelevantTopicIds)
                                item.YoutubeVideoId = v.Id;
                        });
                    }
                );
            });

            await using var dbClient = await TestDatabaseClient.Create();

            await dbClient.UseIt(
                async db =>
                {
                    db.Add(trackUserProps);
                    await db.SaveChangesAsync();
                },
                async db =>
                {
                    var trackUserPropsFromDb = await db.Set<TrackUserProps>()
                        .Include(t => t.YoutubeVideo)
                        .ThenInclude(v => v.Statistics)
                        .Include(t => t.YoutubeVideo)
                        .ThenInclude(v => v.Tags)
                        .Include(t => t.YoutubeVideo)
                        .ThenInclude(v => v.Thumbnails)
                        .Include(t => t.YoutubeVideo)
                        .ThenInclude(v => v.TopicDetails)
                        .ThenInclude(td => td.RelevantTopicIds)
                        .Include(t => t.YoutubeVideo)
                        .ThenInclude(v => v.TopicDetails)
                        .ThenInclude(td => td.TopicIds)
                        .Include(t => t.YoutubeVideo)
                        .ThenInclude(v => v.TopicDetails)
                        .ThenInclude(td => td.TopicCategories)
                        .Include(t => t.YoutubeVideo)
                        .ThenInclude(v => v.YoutubeChannel)
                        .Include(t => t.TrackTags)
                        .Include(t => t.User)
                        .SingleAsync();

                    trackUserPropsFromDb.Id.Should().BeGreaterThan(0);
                    trackUserPropsFromDb.UserId.Should().Be(trackUserProps.User.Id);

                    trackUserPropsFromDb.Should().BeEquivalentTo(trackUserProps,
                        o => o.Excluding(v => v.YoutubeVideo.TrackUserProps)
                    );

                    trackUserProps.YoutubeVideo.TopicDetails.Id.Should().BeGreaterThan(0);
                    trackUserProps.YoutubeVideo.TopicDetails.YoutubeVideoId.Should().Be(
                        trackUserProps.YoutubeVideo.Id
                    );
                }
            );
        }

        [Fact]
        public async Task Can_Add_Full_YouTubeVideo()
        {
            var ytVideo = _gen.YoutubeVideo(
                v =>
                {
                    v.Id = _gen.String();
                    v.Statistics = _gen.YoutubeVideoStatistics();
                    v.Tags = new[]
                    {
                        _gen.YoutubeVideoTag(),
                        _gen.YoutubeVideoTag(),
                    };
                    v.Thumbnails = new[]
                    {
                        _gen.YoutubeVideoThumbnail(),
                        _gen.YoutubeVideoThumbnail(),
                    };
                    v.YoutubeChannelId = _gen.String();
                    v.YoutubeChannel = _gen.YoutubeChannel(
                        c => { c.Id = v.YoutubeChannelId; }
                    );
                    v.TopicDetails = _gen.YoutubeVideoTopicDetails(td =>
                    {
                        foreach (var item in td.TopicCategories)
                            item.YoutubeVideoId = v.Id;
                        foreach (var item in td.TopicIds)
                            item.YoutubeVideoId = v.Id;
                        foreach (var item in td.RelevantTopicIds)
                            item.YoutubeVideoId = v.Id;
                    });
                    v.TrackUserProps = new[]
                    {
                        _gen.Track(t =>
                        {
                            t.User = _gen.User();
                            t.TrackTags = new[]
                            {
                                _gen.TrackTag(),
                                _gen.TrackTag(),
                            };
                        }),
                        _gen.Track(t =>
                        {
                            t.User = _gen.User();
                            t.TrackTags = new[]
                            {
                                _gen.TrackTag(),
                            };
                        }),
                    };
                }
            );

            await using var dbClient = await TestDatabaseClient.Create();

            await dbClient.UseIt(
                async db =>
                {
                    db.Add(ytVideo);
                    await db.SaveChangesAsync();
                },
                async db =>
                {
                    var ytVideoFromDb = await db.YoutubeVideos
                        .Include(v => v.Statistics)
                        .Include(v => v.Tags)
                        .Include(v => v.Thumbnails)
                        .Include(v => v.TopicDetails)
                        .ThenInclude(td => td.RelevantTopicIds)
                        .Include(v => v.TopicDetails)
                        .ThenInclude(td => td.TopicIds)
                        .Include(v => v.TopicDetails)
                        .ThenInclude(td => td.TopicCategories)
                        .Include(v => v.YoutubeChannel)
                        .Include(v => v.TrackUserProps)
                        .ThenInclude(td => td.TrackTags)
                        .SingleAsync();

                    ytVideo.Should().BeEquivalentTo(
                        ytVideoFromDb,
                        o => o.Excluding(v => v.Id)
                            .Excluding(v => v.Statistics.Id)
                            .Excluding(v => v.Statistics.YoutubeVideoId)
                            .Excluding(v => v.Tags)
                            .Excluding(v => v.Thumbnails)
                            .Excluding(v => v.TopicDetails.Id)
                            .Excluding(v => v.TopicDetails.YoutubeVideoId)
                            .Excluding(v => v.TrackUserProps)
                    );

                    ytVideoFromDb.TrackUserProps.SelectMany(t => t.TrackTags.Select(tt => tt.Value)).Should().BeEquivalentTo(
                        ytVideo.TrackUserProps.SelectMany(t => t.TrackTags.Select(tt => tt.Value))
                    );

                    ytVideoFromDb.TopicDetails.Id.Should().BeGreaterThan(0);
                    ytVideoFromDb.TopicDetails.YoutubeVideoId.Should().Be(
                        ytVideo.Id
                    );
                }
            );
        }
    }
}
