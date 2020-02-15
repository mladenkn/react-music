using Microsoft.Extensions.DependencyInjection;
using Music.DataAccess;
using Music.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Music.Domain
{
    public static class Initializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetRequiredService<MusicDbContext>();
            
            db.Add(new User
            {
                Email = "mladen.knezovic.1993@gmail.com",
            });
            db.SaveChanges();

            await PersistTracks(serviceProvider);
        }

        public static async Task PersistTracks(IServiceProvider serviceProvider)
        {
            var tracks = new[]
            {
                new SaveTrackModel
                {
                    TrackYtId = "https://www.youtube.com/watch?v=4IR5huaJHgU",
                    Tags = new[] {"trance"},
                    Year = 1994
                },
                new SaveTrackModel
                {
                    TrackYtId = "https://www.youtube.com/watch?v=-LpsjHNMtIc",
                    Tags = new[] {"trance", "house", "ambient"},
                    Year = 1992
                },
                new SaveTrackModel
                {
                    TrackYtId = "https://www.youtube.com/watch?v=faFioXZRMXQ",
                    Tags = new[] {"trance", "techno"},
                    Year = 1995
                },
            };

            Normalize(tracks);
            var trackYouTubeVideoIds = tracks.Select(t => t.TrackYtId);
            await serviceProvider.GetRequiredService<PersistYouTubeVideosExecutor>().Execute(trackYouTubeVideoIds);

            foreach (var track in tracks)
            {
                var executor = serviceProvider.GetRequiredService<SaveTrackYoutubeExecutor>();
                await executor.Execute(track);
            }
        }

        public static void Normalize(IEnumerable<SaveTrackModel> tracks)
        {
            foreach (var track in tracks)
            {
                var isIdUri = Uri.TryCreate(track.TrackYtId, UriKind.RelativeOrAbsolute, out var trackUri);
                if (isIdUri) 
                    track.TrackYtId = HttpUtility.ParseQueryString(trackUri.Query).Get("v");
            }
        }
    }
}
