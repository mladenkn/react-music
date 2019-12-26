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
        private readonly DataGenerator _dataGenerator = new DataGenerator();

        [Fact]
        public async Task Can_Add_One_YoutubeChannel()
        {
            var channel = _dataGenerator.YoutubeChannel(
                parts => parts.Add(c => c.Id)
            );

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
        public async Task Cannot_Add_TrackTag_When_No_Such_Track_Exists()
        {
            var trackUserPropsTag = _dataGenerator.TrackTag(
                parts => parts.Add(t => t.TrackId)    
            );

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
        public async Task Cannot_Add_Track_When_No_Such_YoutubeVideo_Exists()
        {
            var user = _dataGenerator.User(
                parts => parts.Add(u => u.Id)    
            );

            using (var services = new Services(DatabaseType.SqlServer))
            {
                services.DbContext.Database.EnsureDeleted();
                services.DbContext.Database.EnsureCreated();

                services.DbContext.Add(user);
                await services.DbContext.SaveChangesAsync();

                var trackUserProps = new Track
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
            var video = _dataGenerator.YoutubeVideo(
                parts => parts.Add(v => v.Id),
                v => { v.YoutubeChannel = _dataGenerator.YoutubeChannel(parts => parts.Add(c => c.Id)); }
            );

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
        public async Task Can_Add_Full_Track_With_Full_YouTubeVideo()
        {
            var video = _dataGenerator.YoutubeVideo(
                parts => parts.Add(v => v.Id).Add(v => v.YoutubeChannelId),
                v =>
                {
                    v.Statistics = _dataGenerator.YoutubeVideoStatistics();
                    v.Tags = new[]
                    {
                        _dataGenerator.YoutubeVideoTag(),
                        _dataGenerator.YoutubeVideoTag(),
                    };
                    v.Thumbnails = new[]
                    {
                        _dataGenerator.YoutubeVideoThumbnail(),
                        _dataGenerator.YoutubeVideoThumbnail(),
                    };
                    v.YoutubeChannel = _dataGenerator.YoutubeChannel(
                        parts => { },
                        c => { c.Id = v.YoutubeChannelId; }
                    );
                    v.TopicDetails = _dataGenerator.YoutubeVideoTopicDetails();
                }
            );

            var trackUserProps = new Track
            {
                TrackTags = new[]
                {
                    new TrackTag {Value = Guid.NewGuid().ToString()},
                    new TrackTag {Value = Guid.NewGuid().ToString()},
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
                var trackUserPropsFromDb = await services.DbContext.Set<Track>()
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

                services.DbContext.Database.EnsureDeleted();
            }
        }
    }
}
