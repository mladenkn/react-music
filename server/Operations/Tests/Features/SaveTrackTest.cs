﻿using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Executables.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Music.DataAccess;
using Music.DataAccess.Models;
using Music.Domain;
using Music.Domain.Shared;
using Utilities;
using Xunit;

namespace Executables.Tests.Features
{
    public class SaveTrackTest
    {
        private readonly DataGenerator _gen = new DataGenerator();

        [Fact]
        public async Task CanDeleteOneTagAndAddOneTag()
        {
            await ServerTest.Run(options =>
            {
                var track = _gen.Track(t =>
                {
                    t.User = _gen.User();
                    t.YoutubeVideo = _gen.YoutubeVideo(v =>
                    {
                        v.Id = _gen.String();
                        v.YoutubeChannel = _gen.YoutubeChannel(c => { c.Id = _gen.String(); });
                    });
                    t.TrackTags = new[]
                    {
                        new TrackUserPropsTag { Value = "1" },
                        new TrackUserPropsTag { Value = "2" },
                    };
                });

                var saveTrackModel = new SaveTrackModel
                {
                    Tags = new[] { "2", "3" },
                    Year = _gen.Int(),
                    TrackYtId = track.YoutubeVideo.Id,
                };

                options
                    .PrepareDatabase(db =>
                    {
                        db.Add(track);
                        db.SaveChanges();
                    })
                    .ConfigureServices(services => ConfigureServices(services, track.User.Id))
                    .Act(httpClient => httpClient.PostJsonAsync("api/tracks", saveTrackModel))
                    .Assert(async (serverResponse, db) => Assert(serverResponse, db, saveTrackModel));
            });
        }

        [Fact]
        public async Task CanAdd()
        {
            await ServerTest.Run(options =>
            {
                var youtubeVideo = _gen.YoutubeVideo(v =>
                {
                    v.Id = _gen.String();
                    v.YoutubeChannel = _gen.YoutubeChannel(c => { c.Id = _gen.String(); });
                });

                var user = _gen.User();

                var saveTrackModel = new SaveTrackModel
                {
                    Tags = new[] { _gen.String(), _gen.String() },
                    Year = _gen.Int(),
                    TrackYtId = youtubeVideo.Id,
                };

                options
                    .PrepareDatabase(db =>
                    {
                        db.Add(youtubeVideo);
                        db.Add(user);
                        db.SaveChanges();
                    })
                    .ConfigureServices(services => ConfigureServices(services, user.Id))
                    .Act(httpClient => httpClient.PostJsonAsync("api/tracks", saveTrackModel))
                    .Assert(async (serverResponse, db) => Assert(serverResponse, db, saveTrackModel));
            });
        }

        [Fact]
        public async Task FailsWhenTryingToUpdateOtherUsersTrack()
        {
            await ServerTest.Run(options =>
            {
                var tracksUser = _gen.User();
                var otherUser = _gen.User();

                var track = _gen.Track(t =>
                {
                    t.User = tracksUser;
                    t.YoutubeVideo = _gen.YoutubeVideo(v =>
                    {
                        v.Id = _gen.String();
                        v.YoutubeChannel = _gen.YoutubeChannel(c => { c.Id = _gen.String(); });
                    });
                    t.YoutubeVideoId = t.YoutubeVideo.Id;
                });

                var saveTrackModel = new SaveTrackModel
                {
                    Tags = new string[0],
                    TrackYtId = track.YoutubeVideoId,
                };

                options
                    .PrepareDatabase(db =>
                    {
                        db.Add(tracksUser);
                        db.Add(otherUser);
                        db.Add(track);
                        db.SaveChanges();
                    })
                    .ConfigureServices(services => ConfigureServices(services, otherUser.Id))
                    .Act(httpClient => httpClient.PostJsonAsync("api/tracks", saveTrackModel))
                    .Assert(async (response, db) =>
                    {
                        var trackFromDb = db.TrackUserProps.Single();
                        trackFromDb.Year.Should().Be(track.Year);
                        response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.BadRequest);
                    });
            });
        }

        private void ConfigureServices(IServiceCollection services, int userId)
        {
            var currentUserContext = new Mock<ICurrentUserContext>();
            currentUserContext.Setup(c => c.Id).Returns(userId);
            services.AddTransient<ICurrentUserContext>(sp => currentUserContext.Object);
        }

        private void Assert(HttpResponseMessage serverResponse, MusicDbContext db, SaveTrackModel saveTrackModel)
        {
            serverResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var readTrack = db.TrackUserProps.Include(t => t.TrackTags).Single();
            var shouldBeTrackProps = new
            {
                YoutubeVideoId = saveTrackModel.TrackYtId,
                saveTrackModel.Year,
                saveTrackModel.Tags
            };
            var readTrackPropsToCompare = new
            {
                Tags = readTrack.TrackTags.Select(t => t.Value),
                readTrack.Year,
                readTrack.YoutubeVideoId
            };
            shouldBeTrackProps.Should().BeEquivalentTo(readTrackPropsToCompare);
        }
    }
}