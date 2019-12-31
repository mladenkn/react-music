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
        public void Can_Add_One_YoutubeChannel()
        {
            var channel = _gen.YoutubeChannel(c => { c.Id = _gen.String(); });

            using (var db = Utils.UseDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Add(channel);
                db.SaveChanges();
            }

            using (var db = Utils.UseDbContext())
            {
                var channelFromDb = db.Set<YoutubeChannel>().Single();
                channelFromDb.Id.Should().Be(channel.Id);
                channelFromDb.Title.Should().Be(channel.Title);

                db.Database.EnsureDeleted();
            }
        }
        
        [Fact]
        public void Can_Add_YoutubeVideo()
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
                db.SaveChanges();
            }

            using (var db = Utils.UseDbContext())
            {
                var videoFromDb = db.Set<YoutubeVideo>().Include(v => v.YoutubeChannel).Single();
                videoFromDb.Should().BeEquivalentTo(video);
                db.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void Can_Add_Full_Track_With_Full_YouTubeVideo()
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

            using (var db = Utils.UseDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Add(trackUserProps);
                db.SaveChanges();
            }

            using (var db = Utils.UseDbContext())
            {
                var trackUserPropsFromDb = db.Set<TrackUserProps>()
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
                    .Single();

                trackUserPropsFromDb.Id.Should().BeGreaterThan(0);
                trackUserPropsFromDb.UserId.Should().Be(trackUserProps.User.Id);

                trackUserPropsFromDb.Should().BeEquivalentTo(trackUserProps, 
                    o => o.Excluding(v => v.YoutubeVideo.TrackUserProps)
                );

                trackUserProps.YoutubeVideo.TopicDetails.Id.Should().BeGreaterThan(0);
                trackUserProps.YoutubeVideo.TopicDetails.YoutubeVideoId.Should().Be(
                    trackUserProps.YoutubeVideo.Id
                );

                db.Database.EnsureDeleted();
            }
        }
    }
}
