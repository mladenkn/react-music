using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;
using Kernel;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess;
using Music.DataAccess.Models;
using Music.Domain.Shared;
using Utilities;

namespace Music.Domain.QueryTracksViaYoutube
{
    public class QueryTracksViaYoutubeExecutor : ServiceResolverAware<MusicDbContext>
    {
        public QueryTracksViaYoutubeExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<TrackModel>> Execute(string searchQuery)
        {
            var searchYoutubeVideosIds = Resolve<SearchYoutubeVideosIds>();
            var wantedTracksYtIds = (await searchYoutubeVideosIds(searchQuery)).ToArray();

            var notFoundVideosIds = await FilterToUnknownVideosIds(wantedTracksYtIds);
            var videosFromYt = await GetVideosFromYoutube(notFoundVideosIds.ToArray());
            var videosFromYtMapped = videosFromYt.Select(v => Mapper.Map<YoutubeVideo>(v));
            Db.AddRange(videosFromYtMapped);
            await Db.SaveChangesAsync();

            var tracks = await GetTracks(wantedTracksYtIds);

            return tracks;
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

        private async Task<IReadOnlyCollection<Video>> GetVideosFromYoutube(IReadOnlyCollection<string> ids)
        {
            var r = new List<Video>(ids.Count);
            var listVideos = Resolve<ListYoutubeVideos>();

            foreach (var idsChunk in ids.Batch(50))
            {
                var allVideosFromYt = await listVideos(new [] {"snippet","contentDetails","statistics","topicDetails"}, idsChunk);
                r.AddRange(allVideosFromYt);
            }

            return r;
        }

        private async Task<IReadOnlyCollection<TrackModel>> GetTracks(IEnumerable<string> wantedTracksYtIds)
        {
            var curUserId = Resolve<ICurrentUserContext>().Id;
            var tracks = await Db.YoutubeVideos
                .Where(v => wantedTracksYtIds.Contains(v.Id))
                .Select(TrackModel.FromYoutubeVideo(curUserId))
                .ToListAsync();
            return tracks;
        }
    }
}
