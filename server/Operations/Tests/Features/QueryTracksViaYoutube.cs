using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Executables.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Music.Domain.QueryTracksViaYoutube;
using Music.Domain.Shared;
using Newtonsoft.Json;
using Utilities;
using Xunit;

namespace Executables.Tests.Features
{
    public class QueryTracksViaYoutube
    {
        private readonly DataGenerator _gen = new DataGenerator();

        [Fact]
        public async Task Run()
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
                _gen.YoutubeVideoModel(v =>
                {
                    v.Id = searchedVideoIds[1];
                    v.ChannelId = _gen.String();
                }),
                _gen.YoutubeVideoModel(v =>
                {
                    v.Id = searchedVideoIds[2];
                    v.ChannelId = _gen.String();
                }),
            };

            var shouldBeVideoIdsInDbAtTheEnd = Enumerable.Union(searchedVideoIds, videosInDb.Select(v => v.Id));

            await ServerTest.Run(options =>
            {
                options.ConfigureServices(services =>
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
    }
}