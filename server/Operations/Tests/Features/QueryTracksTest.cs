using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Executables.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Music.DataAccess.Models;
using Music.Domain.Shared;
using Utilities;
using Xunit;

namespace Executables.Tests.Features
{
    public class QueryTracksTest
    {
        private readonly DataGenerator _gen = new DataGenerator();

        [Fact]
        public async Task ShouldTakeOnlyUsersTracks()
        {
            var activeUser = _gen.User();
            var otherUser = _gen.User();

            var activeUserTracks = new[]
            {
                _gen.Track(t =>
                {
                    t.YoutubeVideo = GenerateYoutubeVideo();
                    t.User = activeUser;
                    t.TrackTags = new[]
                    {
                        _gen.TrackTag(),
                        _gen.TrackTag(),
                    };
                }),
                _gen.Track(t =>
                {
                    t.YoutubeVideo = GenerateYoutubeVideo();
                    t.User = activeUser;
                    t.TrackTags = new[]
                    {
                        _gen.TrackTag(),
                    };
                }),
                _gen.Track(t =>
                {
                    t.YoutubeVideo = GenerateYoutubeVideo();
                    t.User = activeUser;
                    t.TrackTags = new TrackUserPropsTag[0];
                    t.Year = null;
                }),
            };

            var otherUserTracks = new []
            {
                _gen.Track(t =>
                {
                    t.YoutubeVideo = GenerateYoutubeVideo();
                    t.User = otherUser;
                    t.TrackTags = new TrackUserPropsTag[0];
                }),
                _gen.Track(t =>
                {
                    t.YoutubeVideo = GenerateYoutubeVideo();
                    t.User = otherUser;
                    t.Year = null;
                    t.TrackTags = new[]
                    {
                        _gen.TrackTag(),
                        _gen.TrackTag(),
                        _gen.TrackTag(),
                    };
                }),
            };

            TrackModel TrackModelMatcherOfIndex(int index)
            {
                var trackUserProps = activeUserTracks[index];
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

            await ServerTest.Run(options =>
            {
                options
                    .PrepareDatabase(async db =>
                    {
                        db.AddRange(activeUserTracks);
                        db.AddRange(otherUserTracks);
                        await db.SaveChangesAsync();
                    })
                    .ConfigureServices(services =>
                    {
                        var currentUserContext = new Mock<ICurrentUserContext>();
                        currentUserContext.Setup(c => c.Id).Returns(activeUser.Id);
                        services.AddTransient<ICurrentUserContext>(sp => currentUserContext.Object);
                    })
                    .Act(httpClient => httpClient.GetAsync("/api/tracks?skip=0&take=10"))
                    .Assert(async (response, db) =>
                    {
                        response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.OK);
                        var responseContent = await response.Content.ParseAsJson<ArrayWithTotalCount<TrackModel>>();
                        var shouldBeResponseTracks = new[]
                        {
                            TrackModelMatcherOfIndex(0),
                            TrackModelMatcherOfIndex(1),
                            TrackModelMatcherOfIndex(2)
                        };
                        responseContent.Data.Should().BeEquivalentTo(shouldBeResponseTracks);
                    });
            });
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
