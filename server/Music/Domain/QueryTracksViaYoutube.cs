using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.Domain.Shared;
using Music.Domain.Shared.Models;
using Music.Domain.YouTubeVideos;

namespace Music.Domain
{
    public class QueryTracksViaYoutubeExecutor : ServiceResolverAware
    {
        public QueryTracksViaYoutubeExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<TrackModel>> Execute(string query)
        {
            var wantedTracksYtIds = await Resolve<YouTubeVideoService>().SearchIds(query);
            await Resolve<PersistYouTubeVideosIfFoundExecutor>().Execute(wantedTracksYtIds);
            var tracks = await GetTracks(wantedTracksYtIds);
            return tracks;
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
