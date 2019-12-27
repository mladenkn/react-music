using System.Linq;
using System.Threading.Tasks;
using Executables.Helpers;
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

            using (var db = Utils.UseDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Add(channel);
                await db.SaveChangesAsync();
            }

            using (var db = Utils.UseDbContext())
            {
                var channelQueryResult = await db.Set<YoutubeChannel>().ToArrayAsync();
                Assert.Single(channelQueryResult);
                Assert.Equal(channel.Id, channelQueryResult[0].Id);
                Assert.Equal(channel.Title, channelQueryResult[0].Title);

                db.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Cannot_Add_TrackTag_When_No_Such_Track_Exists()
        {
            var trackUserPropsTag = _gen.TrackTag(t => { t.TrackId = _gen.Int(); });

            using (var db = Utils.UseDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Add(trackUserPropsTag);
                await Assert.ThrowsAnyAsync<DbUpdateException>(() => db.SaveChangesAsync());

                db.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Cannot_Add_Track_When_No_Such_YoutubeVideo_Exists()
        {
            var user = _gen.User();

            using (var db = Utils.UseDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Add(user);
                await db.SaveChangesAsync();

                var trackUserProps = new Track
                {
                    UserId = 1,
                    YoutubeVideoId = "1",
                };

                db.Add(trackUserProps);
                await Assert.ThrowsAnyAsync<DbUpdateException>(() => db.SaveChangesAsync());

                db.Database.EnsureDeleted();
            }
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

            using (var db = Utils.UseDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Add(video);
                await db.SaveChangesAsync();
            }

            using (var db = Utils.UseDbContext())
            {
                var videoFromDb = await db.Set<YoutubeVideo>().SingleAsync();

                Assert.Equal(video.Id, videoFromDb.Id);
                Assert.Equal(video.Description, videoFromDb.Description);
                Assert.Equal(video.Duration, videoFromDb.Duration);
                Assert.Equal(video.ThumbnailsEtag, videoFromDb.ThumbnailsEtag);
                Assert.Equal(video.YoutubeCategoryId, videoFromDb.YoutubeCategoryId);
                Assert.Equal(video.Title, videoFromDb.Title);
                Assert.Equal(video.YoutubeChannelId, videoFromDb.YoutubeChannelId);

                db.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Can_Add_Full_Track_With_Full_YouTubeVideo()
        {
            var video = _gen.YoutubeVideo(
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
                    v.TopicDetails = _gen.YoutubeVideoTopicDetails();
                }
            );

            var trackUserProps = _gen.Track(t =>
            {
                t.TrackTags = new[]
                {
                    _gen.TrackTag(),
                    _gen.TrackTag(),
                };
                t.User = _gen.User();
                t.YoutubeVideo = video;
            });

            using (var db = Utils.UseDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Add(trackUserProps);
                await db.SaveChangesAsync();
            }

            using (var db = Utils.UseDbContext())
            {
                var trackUserPropsFromDb = await db.Set<Track>()
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
                        .ThenInclude(v => v.YoutubeChannel)
                    .Include(t => t.TrackTags)
                    .SingleAsync();

                Assert.True(trackUserPropsFromDb.Id > 0);
                Assert.Equal(trackUserProps.User.Id, trackUserPropsFromDb.UserId);
                Assert.Equal(trackUserProps.Year, trackUserPropsFromDb.Year);
                Assert.Equal(trackUserProps.YoutubeVideoId, trackUserPropsFromDb.YoutubeVideo.Id);

                Assert.True(Enumerable.SequenceEqual(
                    trackUserProps.TrackTags.Select(t => t.Value).OrderBy(t => t),
                    trackUserPropsFromDb.TrackTags.Select(t => t.Value).OrderBy(t => t)
                ));
                
                Assert.Equal(video.Id, trackUserPropsFromDb.YoutubeVideo.Id);
                Assert.Equal(video.Description, trackUserPropsFromDb.YoutubeVideo.Description);
                Assert.Equal(video.Duration, trackUserPropsFromDb.YoutubeVideo.Duration);
                Assert.Equal(video.ThumbnailsEtag, trackUserPropsFromDb.YoutubeVideo.ThumbnailsEtag);
                Assert.Equal(video.YoutubeCategoryId, trackUserPropsFromDb.YoutubeVideo.YoutubeCategoryId);
                Assert.Equal(video.Title, trackUserPropsFromDb.YoutubeVideo.Title);
                Assert.Equal(video.YoutubeChannelId, trackUserPropsFromDb.YoutubeVideo.YoutubeChannelId);

                Assert.Equal(video.TopicDetails.ETag, trackUserPropsFromDb.YoutubeVideo.TopicDetails.ETag);
                Assert.True(video.TopicDetails.Id > 0);
                Assert.Equal(video.TopicDetails.YoutubeVideoId, video.Id);

                Assert.True(Enumerable.SequenceEqual(
                    trackUserPropsFromDb.YoutubeVideo.Tags.Select(t => t.Value).OrderBy(t => t),
                    trackUserPropsFromDb.YoutubeVideo.Tags.Select(t => t.Value).OrderBy(t => t)
                ));

                Assert.True(Enumerable.SequenceEqual(
                    trackUserPropsFromDb.YoutubeVideo.TopicDetails.RelevantTopicIds.Select(t => t.Value).OrderBy(t => t),
                    trackUserPropsFromDb.YoutubeVideo.TopicDetails.RelevantTopicIds.Select(t => t.Value).OrderBy(t => t)
                ));

                Assert.True(Enumerable.SequenceEqual(
                    trackUserPropsFromDb.YoutubeVideo.Thumbnails.Select(t => t.Url).OrderBy(t => t),
                    trackUserPropsFromDb.YoutubeVideo.Thumbnails.Select(t => t.Url).OrderBy(t => t)
                ));

                db.Database.EnsureDeleted();
            }
        }
    }
}
