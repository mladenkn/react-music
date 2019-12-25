using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Models;
using Xunit;

namespace Executables
{
    public class DbTest
    {
        [Fact]
        public async Task Can_Add_One_YoutubeChannel()
        {
            var channel = new YoutubeChannel
            {
                Id = "1",
                Title = "channel"
            };

            using (var services = new Services(DatabaseType.SqlServer))
            {
                services.DbContext.Database.EnsureDeleted();
                services.DbContext.Database.EnsureCreated();

                services.DbContext.Add(channel);
                await services.DbContext.SaveChangesAsync();
            }

            using (var services = new Services(DatabaseType.SqlServer))
            {
                var channelQueryResult = await services.DbContext.Set<YoutubeChannel>().ToArrayAsync();
                Assert.Single(channelQueryResult);
                Assert.Equal(channel.Id, channelQueryResult[0].Id);
                Assert.Equal(channel.Title, channelQueryResult[0].Title);

                services.DbContext.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Cannot_Add_TrackUserPropsTag_When_No_Such_TrackUserProps_Exists()
        {
            var trackUserPropsTag = new TrackUserPropsTag
            {
                TrackUserPropsId = 1,
                Value = "tag"
            };

            using (var services = new Services(DatabaseType.SqlServer))
            {
                services.DbContext.Database.EnsureDeleted();
                services.DbContext.Database.EnsureCreated();

                services.DbContext.Add(trackUserPropsTag);
                await Assert.ThrowsAnyAsync<DbUpdateException>(() => services.DbContext.SaveChangesAsync());

                services.DbContext.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Cannot_Add_TrackUserProps_When_No_Such_YoutubeVideo_Exists()
        {
            var user = new User
            {
                Email = "user@gmail.com"
            };

            using (var services = new Services(DatabaseType.SqlServer))
            {
                services.DbContext.Database.EnsureDeleted();
                services.DbContext.Database.EnsureCreated();

                services.DbContext.Add(user);
                await services.DbContext.SaveChangesAsync();

                var trackUserProps = new TrackUserProps
                {
                    UserId = 1,
                    YoutubeVideoId = "1",
                };

                services.DbContext.Add(trackUserProps);
                await Assert.ThrowsAnyAsync<DbUpdateException>(() => services.DbContext.SaveChangesAsync());

                services.DbContext.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Can_Add_YoutubeVideo()
        {
            var video = new YoutubeVideo
            {
                Id = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Duration = TimeSpan.FromSeconds(2),
                ThumbnailsEtag = Guid.NewGuid().ToString(),
                YoutubeCategoryId = Guid.NewGuid().ToString(),
                Title = Guid.NewGuid().ToString(),
                YoutubeChannelId = "YoutubeChannel 1",
                YoutubeChannel = new YoutubeChannel
                {
                    Id = "YoutubeChannel 1",
                    Title = "YoutubeChannel 1",
                }
            };

            using (var services = new Services(DatabaseType.SqlServer))
            {
                services.DbContext.Database.EnsureDeleted();
                services.DbContext.Database.EnsureCreated();

                services.DbContext.Add(video);
                await services.DbContext.SaveChangesAsync();
            }
            
            using (var services = new Services(DatabaseType.SqlServer))
            {
                var videoFromDb = await services.DbContext.Set<YoutubeVideo>().SingleAsync();

                Assert.Equal(video.Id, videoFromDb.Id);
                Assert.Equal(video.Description, videoFromDb.Description);
                Assert.Equal(video.Duration, videoFromDb.Duration);
                Assert.Equal(video.ThumbnailsEtag, videoFromDb.ThumbnailsEtag);
                Assert.Equal(video.YoutubeCategoryId, videoFromDb.YoutubeCategoryId);
                Assert.Equal(video.Title, videoFromDb.Title);
                Assert.Equal(video.YoutubeChannelId, videoFromDb.YoutubeChannelId);

                services.DbContext.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task Can_Add_Full_TrackUserProps_With_Full_YouTubeVideo()
        {
            var video = new YoutubeVideo
            {
                Id = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Duration = TimeSpan.FromSeconds(2),
                Statistics = new YoutubeVideoStatistics
                {
                    CommentCount = 1,
                },
                Tags = new []
                {
                    new YoutubeVideoTag
                    {
                        Value = Guid.NewGuid().ToString(),
                    },
                    new YoutubeVideoTag
                    {
                        Value = Guid.NewGuid().ToString(),
                    }
                },
                Thumbnails = new []
                {
                    new YoutubeVideoThumbnail
                    {
                        Name = Guid.NewGuid().ToString(),
                        Etag = Guid.NewGuid().ToString(),
                        Url = Guid.NewGuid().ToString()
                    },
                    new YoutubeVideoThumbnail
                    {
                        Name = Guid.NewGuid().ToString(),
                        Etag = Guid.NewGuid().ToString(),
                        Url = Guid.NewGuid().ToString()
                    },
                },
                TopicDetails = new YoutubeVideoTopicDetails
                {
                    ETag = Guid.NewGuid().ToString(),
                    RelevantTopicIds = new []
                    {
                        new YoutubeVideoTopicDetailsRelevantTopicId
                        {
                            Value = Guid.NewGuid().ToString(),
                        },
                        new YoutubeVideoTopicDetailsRelevantTopicId
                        {
                            Value = Guid.NewGuid().ToString(),
                        },
                    }
                },
                ThumbnailsEtag = Guid.NewGuid().ToString(),
                YoutubeCategoryId = Guid.NewGuid().ToString(),
                Title = Guid.NewGuid().ToString(),
                YoutubeChannelId = "YoutubeChannel 1",
                YoutubeChannel = new YoutubeChannel
                {
                    Id = "YoutubeChannel 1",
                    Title = "YoutubeChannel 1",
                }
            };

            var trackUserProps = new TrackUserProps
            {
                TrackUserPropsTags = new[]
                {
                    new TrackUserPropsTag {Value = Guid.NewGuid().ToString()},
                    new TrackUserPropsTag {Value = Guid.NewGuid().ToString()},
                },
                User = new User
                {
                    Email = Guid.NewGuid().ToString()
                },
                YoutubeVideo = video,
                Year = 1997,
            };

            using (var services = new Services(DatabaseType.SqlServer))
            {
                services.DbContext.Database.EnsureDeleted();
                services.DbContext.Database.EnsureCreated();

                services.DbContext.Add(trackUserProps);
                await services.DbContext.SaveChangesAsync();
            }
            
            using (var services = new Services(DatabaseType.SqlServer))
            {
                var trackUserPropsFromDb = await services.DbContext.Set<TrackUserProps>()
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
                    .Include(t => t.TrackUserPropsTags)
                    .SingleAsync();

                Assert.True(trackUserPropsFromDb.Id > 0);
                Assert.Equal(trackUserProps.User.Id, trackUserPropsFromDb.UserId);
                Assert.Equal(trackUserProps.Year, trackUserPropsFromDb.Year);
                Assert.Equal(trackUserProps.YoutubeVideoId, trackUserPropsFromDb.YoutubeVideo.Id);

                Assert.True(Enumerable.SequenceEqual(
                    trackUserProps.TrackUserPropsTags.Select(t => t.Value).OrderBy(t => t),
                    trackUserPropsFromDb.TrackUserPropsTags.Select(t => t.Value).OrderBy(t => t)
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

                services.DbContext.Database.EnsureDeleted();
            }
        }
    }
}
