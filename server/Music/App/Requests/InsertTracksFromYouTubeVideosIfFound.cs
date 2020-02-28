using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using Music.App.YouTubeVideos;

namespace Music.App.Requests
{
    public class InsertTracksFromYouTubeVideosIfFound : ServiceResolverAware
    {
        public InsertTracksFromYouTubeVideosIfFound(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<Result> Execute(IReadOnlyCollection<string> wantedVideosIds)
        {
            var unknownVideosIds = (await FilterToUnknownVideosIds(wantedVideosIds)).ToArray();
            var videosFromYt = (await Resolve<YouTubeVideoService>().GetByIds(unknownVideosIds)).ToArray();
            
            var tracks = videosFromYt.Select(v => new Track { YoutubeVideos = new[] {v} }).ToArray();

            var dataPersistor = Resolve<DataPersistor>();
            await dataPersistor.InsertTracks(tracks);

            var notFoundVideosIds = wantedVideosIds.Except(videosFromYt.Select(v => v.Id));

            return new Result
            {
                NotFoundVideoIds = notFoundVideosIds,
                NewTracks = videosFromYt.Select(v => v.Track).ToArray(),
                NewYouTubeVideos = videosFromYt
            };
        }

        private async Task<IEnumerable<string>> FilterToUnknownVideosIds(IEnumerable<string> ids)
        {
            var foundIds = await Db.YoutubeVideos
                .Select(v => v.Id)
                .Where(vId => ids.Contains(vId))
                .ToArrayAsync();
            var notFoundIds = ids.Except(foundIds);
            return notFoundIds;
        }


        public class Result
        {
            public IEnumerable<string> NotFoundVideoIds { get; set; }
            public IReadOnlyCollection<Track> NewTracks { get; set; }
            public IReadOnlyCollection<YoutubeVideo> NewYouTubeVideos { get; set; }
        }
    }
}
