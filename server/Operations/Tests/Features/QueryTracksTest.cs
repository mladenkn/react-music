using System.Linq;
using System.Threading.Tasks;
using Executables.Helpers;
using Music.Domain;
using Utilities;
using Xunit;

namespace Executables.Tests.Features
{
    public class QueryTracksTest
    {
        //[Fact]
        public async Task Run()
        {
            var gen = new DataGenerator();

            var ytVideosInDb = CollectionUtils.Repeat(() =>
                gen.YoutubeVideo(v =>
                {
                    v.Id = gen.String();
                    v.YoutubeChannel = gen.YoutubeChannel(c => { c.Id = gen.String(); });
                    v.Thumbnails = new[]
                    {
                        gen.YoutubeVideoThumbnail(t => { t.Name = "Default__"; }),
                        gen.YoutubeVideoThumbnail(),
                    };
                }),
                3
            );

            var request = new QueryTracksRequest
            {
                MustHaveEveryTag = new []{ gen.String(), gen.String() },
                MustHaveAnyTag = new[] { gen.String(), gen.String() },
                YearRange = new Range<int>
                {
                    LowerBound = 1990,
                    UpperBound = 2000,
                }
            };

            var tracksInDb = new[]
            {
                gen.Track(t =>
                {
                    t.YoutubeVideoId = ytVideosInDb[0].Id;
                    t.TrackTags = new[]
                    {
                        gen.TrackTag(),
                        gen.TrackTag(),
                    };
                    t.Year = null;
                    t.User = gen.User(u => u.Id = gen.Int());
                }),
                gen.Track(t =>
                {
                    t.YoutubeVideoId = ytVideosInDb[0].Id;
                    t.TrackTags = new[]
                    {
                        gen.TrackTag(),
                        gen.TrackTag(),
                    };
                    t.User = gen.User(u => u.Id = gen.Int());
                }),
                gen.Track(t =>
                {
                    t.YoutubeVideoId = ytVideosInDb[0].Id;
                    t.User = gen.User(u => u.Id = gen.Int());
                }),
            };

            await ServerTest.Run(options =>
            {
                options
                    .PrepareDatabase(db =>
                    {
                        db.Add(ytVideosInDb);
                        db.Add(tracksInDb);
                        db.SaveChanges();
                    })
                    .Act(httpClient => httpClient.GetAsync("api/tracks/"))
                    .Assert(async (serverResponse, db) =>
                    {

                    });
            });
        }
    }
}
