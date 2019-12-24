using System;
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
        public async Task Can_Add_YoutubeVideoWithRelationProperties()
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
                        new YoutubeVideoTopicDetailsRelevantTopicIds
                        {
                            Value = Guid.NewGuid().ToString(),
                        },
                        new YoutubeVideoTopicDetailsRelevantTopicIds
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

            using (var services = new Services(DatabaseType.SqlServer))
            {
                services.DbContext.Database.EnsureDeleted();
                services.DbContext.Database.EnsureCreated();

                services.DbContext.Add(video);
                await services.DbContext.SaveChangesAsync();
            }
            
            using (var services = new Services(DatabaseType.SqlServer))
            {
                var videoFromDb = await services.DbContext.Set<YoutubeVideo>()
                    .Include(v => v.TopicDetails)
                    .Include(v => v.Thumbnails)
                    .Include(v => v.Tags)
                    .Include(v => v.Statistics)
                    .SingleAsync();

                Assert.Equal(video.Id, videoFromDb.Id);
                Assert.Equal(video.Description, videoFromDb.Description);
                Assert.Equal(video.Duration, videoFromDb.Duration);
                Assert.Equal(video.ThumbnailsEtag, videoFromDb.ThumbnailsEtag);
                Assert.Equal(video.YoutubeCategoryId, videoFromDb.YoutubeCategoryId);
                Assert.Equal(video.Title, videoFromDb.Title);
                Assert.Equal(video.YoutubeChannelId, videoFromDb.YoutubeChannelId);

                Assert.Equal(video.TopicDetails.ETag, videoFromDb.TopicDetails.ETag);
                Assert.True(video.TopicDetails.Id > 0);

                services.DbContext.Database.EnsureDeleted();
            }
        }
    }
}
