using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Music.App.Services;

namespace Music.Admin.Services
{
    public partial class DatabaseInit
    {
        public async Task SaveTracks()
        {
            var tracks = new[]
            {
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=4IR5huaJHgU",
                    Tags = new[] {"trance"},
                    Year = 1994
                },
                //new SaveTrackModel
                //{
                //    YouTubeVideoId = "https://www.youtube.com/watch?v=zGIRoyKshYI",
                //    Tags = new[] {"trance"},
                //    Year = 1994
                //},
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=y_zQWx6EIfw&list=LL6Eg23nrpQwIHDUwivSw-iA&index=10&t=0s",
                    Tags = new[] {"trance"},
                    Year = 1996
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=KpUrTnZwYMU&list=LL6Eg23nrpQwIHDUwivSw-iA&index=16&t=0s",
                    Tags = new[] {"trance", "techno", "acid"},
                    Year = 1993
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=Jnx41up169Y&list=LL6Eg23nrpQwIHDUwivSw-iA&index=15&t=0s",
                    Tags = new[] {"techno"},
                    Year = 2004
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=N0tdf4l68yE&list=LL6Eg23nrpQwIHDUwivSw-iA&index=14&t=0s",
                    Tags = new[] { "techno"},
                    Year = 1997
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=x_TEmrBD6KM&list=LL6Eg23nrpQwIHDUwivSw-iA&index=13&t=0s",
                    Tags = new[] {"techno"},
                    Year = 1997
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=mZ1NOZF90Xg&list=LL6Eg23nrpQwIHDUwivSw-iA&index=12&t=0s",
                    Tags = new[] {"progressive", "trance", "house"},
                    Year = 2000
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=Ic2Q-G0BEFQ&list=LL6Eg23nrpQwIHDUwivSw-iA&index=11&t=0s",
                    Tags = new[] {"trance", "leftfield"},
                    Year = 2000
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=MV_3Dpw-BRY&list=LL6Eg23nrpQwIHDUwivSw-iA&index=29&t=0s",
                    Tags = new[] {"house", "electro", "synth-pop"},
                    Year = 2000
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=I1TM76HuLPI&list=LL6Eg23nrpQwIHDUwivSw-iA&index=28&t=0s",
                    Tags = new[] {"house", "deep"},
                    Year = 1998
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=JVJL-xWwdlw&list=LL6Eg23nrpQwIHDUwivSw-iA&index=32&t=0s",
                    Tags = new[] {"breaks", "downtempo"},
                    Year = 1998
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=OCK4jXjtPGo&list=LL6Eg23nrpQwIHDUwivSw-iA&index=34",
                    Tags = new[] {"acid", "techno"},
                    Year = 1993
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=xVXlxd4Qnic&list=LL6Eg23nrpQwIHDUwivSw-iA&index=35",
                    Tags = new[] {"acid", "techno"},
                    Year = 1993
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=0_fqJP-t9k0&list=LL6Eg23nrpQwIHDUwivSw-iA&index=36",
                    Tags = new[] {"techno", "dark", "atmospheric"},
                    Year = 2018
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=ZZXcdK0RUuQ&list=LL6Eg23nrpQwIHDUwivSw-iA&index=38",
                    Tags = new[] {"techno"},
                    Year = 1993
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=RBWMMAe3d9A&list=LL6Eg23nrpQwIHDUwivSw-iA&index=44",
                    Tags = new[] {"house", "deep"},
                    Year = 2018
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=-73nvxBw5k4&list=LL6Eg23nrpQwIHDUwivSw-iA&index=49",
                    Tags = new[] {"house", "deep"},
                    Year = 2017
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=z15lJR79pTg&list=LL6Eg23nrpQwIHDUwivSw-iA&index=60",
                    Tags = new[] { "leftfield", "house"},
                    Year = 2017
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=ZaQLOCU9J60&list=LL6Eg23nrpQwIHDUwivSw-iA&index=65",
                    Tags = new[] { "house", "deep" },
                    Year = 2017
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=FaYAg5rZnIY&list=LL6Eg23nrpQwIHDUwivSw-iA&index=66",
                    Tags = new[] { "house", "deep" },
                    Year = 2020
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=CyBliS2R4OY&list=LL6Eg23nrpQwIHDUwivSw-iA&index=67",
                    Tags = new[] { "house", "deep" },
                    Year = 2019
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=Sd0u7-qP2I0&list=LL6Eg23nrpQwIHDUwivSw-iA&index=71",
                    Tags = new[] { "techno", "electro", "acid" },
                    Year = 2019
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=5k6wmU8kTg4",
                    Tags = new[] { "electro" },
                    Year = 2019
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=oUhQx3Hx7Nk&list=LL6Eg23nrpQwIHDUwivSw-iA&index=75",
                    Tags = new[] { "house", "deep" },
                    Year = 2019
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=Olo6QhMSnqo&t=562s",
                    Tags = new[] { "techno", "downtempo", "ambient" },
                    Year = 1994
                },
                //new SaveTrackModel
                //{
                //    YouTubeVideoId = "https://www.youtube.com/watch?v=RfEjtL7JJiQ&list=LL6Eg23nrpQwIHDUwivSw-iA&index=110",
                //    Tags = new[] { "trance",  },
                //    Year = 2000
                //},
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=9ra0Bgf04FE",
                    Tags = new[] { "techno", },
                    Year = 1999
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=PcIJ58chDdA",
                    Tags = new[] { "trance", },
                    Year = 1998
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=GmLF0VuCzTg&list=LL6Eg23nrpQwIHDUwivSw-iA&index=129",
                    Tags = new[] { "techno", },
                    Year = 2018
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=dwC35znyyBY&list=LL6Eg23nrpQwIHDUwivSw-iA&index=131",
                    Tags = new[] { "techno", "industrial", "acid" },
                    Year = 2017
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=1CRZp7Gqom0&list=LL6Eg23nrpQwIHDUwivSw-iA&index=133",
                    Tags = new[] { "progressive", "house", },
                    Year = 2011
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=dQXzMR_gCsc&list=LL6Eg23nrpQwIHDUwivSw-iA&index=134",
                    Tags = new[] { "techno", "house", },
                    Year = 2016
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=pDBQrTJ_OJY&list=LL6Eg23nrpQwIHDUwivSw-iA&index=130",
                    Tags = new[] { "trance", "hard", },
                    Year = 1994
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=7UIHl0oJEpg&list=LL6Eg23nrpQwIHDUwivSw-iA&index=140",
                    Tags = new[] { "classical", },
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=yjLR2NCLq24&list=LL6Eg23nrpQwIHDUwivSw-iA&index=146",
                    Tags = new string[0],
                    Year = 1999,
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=MXQHysTTdfk&list=LL6Eg23nrpQwIHDUwivSw-iA&index=149",
                    Tags = new []{ "techno" },
                    Year = 2004,
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=cDp_iihLt0E&list=LL6Eg23nrpQwIHDUwivSw-iA&index=150",
                    Tags = new []{ "techno" },
                    Year = 2017,
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=NRQAiMysk9o&list=LL6Eg23nrpQwIHDUwivSw-iA&index=152",
                    Tags = new string[0],
                    Year = 2014,
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=4vXOsT3dZwQ&list=LL6Eg23nrpQwIHDUwivSw-iA&index=156",
                    Tags = new []{ "techno", "trance" },
                    Year = 2018,
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=xjCb4CB-hdw&list=LL6Eg23nrpQwIHDUwivSw-iA&index=153",
                    Tags = new []{ "techno", "atmospheric", "deep", "industrial" },
                    Year = 2016,
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=bMvHH_ZYRMg&list=LL6Eg23nrpQwIHDUwivSw-iA&index=159",
                    Tags = new []{ "trance", "hard", },
                    Year = 1995,
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=MucwsnRWllQ&list=LL6Eg23nrpQwIHDUwivSw-iA&index=165",
                    Tags = new []{ "techno", "acid", "hard", },
                    Year = 2011,
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=XjbpHhWwIkQ&list=LL6Eg23nrpQwIHDUwivSw-iA&index=166",
                    Tags = new []{ "progressive", "trance", "house", },
                    Year = 2002,
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=Lx-tt1XBaVM",
                    Tags = new []{ "techno", "acid", },
                    Year = 1993,
                },
                new SaveTrackModel
                {
                    YouTubeVideoId = "https://www.youtube.com/watch?v=pAwWzgUbLag",
                    Tags = new []{ "techno", "acid", },
                    Year = 1993,
                },
            };

            await PersistTracks(tracks);
        }

        private async Task PersistTracks(IReadOnlyCollection<SaveTrackModel> tracks)
        {
            Normalize(tracks);
            var trackYouTubeVideoIds = tracks.Select(t => t.YouTubeVideoId).ToArray();

            var service = Resolve<TracksService>();
            var newVideos = await Resolve<YouTubeVideosService>().EnsureAreSavedIfFound(trackYouTubeVideoIds);
            var newTracks = await service.SaveTracksFromVideos(newVideos);

            foreach (var newTrackDescriptor in tracks)
            {
                var track = newTracks.Single(t => t.YoutubeVideos.First().Id == newTrackDescriptor.YouTubeVideoId);
                var saveTrackModel = new SaveTrackUserPropsModel
                {
                    TrackId = track.Id,
                    Tags = newTrackDescriptor.Tags,
                    Year = newTrackDescriptor.Year
                };
                await service.SaveUserProps(saveTrackModel);
            }
        }

        private void Normalize(IEnumerable<SaveTrackModel> tracks)
        {
            foreach (var track in tracks)
            {
                var isIdUri = Uri.TryCreate(track.YouTubeVideoId, UriKind.RelativeOrAbsolute, out var trackUri);
                if (isIdUri)
                    track.YouTubeVideoId = HttpUtility.ParseQueryString(trackUri.Query).Get("v");
            }
        }
    }
    public class SaveTrackModel
    {
        public string YouTubeVideoId { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }

        public int Year { get; set; }
    }
}
