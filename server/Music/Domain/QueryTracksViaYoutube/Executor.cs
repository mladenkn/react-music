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
            await Resolve<PersistYouTubeVideosExecutor>().Execute(wantedTracksYtIds);
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
