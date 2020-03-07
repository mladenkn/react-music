using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using Music.App.Models;

namespace Music.App.Services
{
    public class QueryTracksViaYoutubeExecutor : ServiceResolverAware
    {
        public QueryTracksViaYoutubeExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<TrackForHomeSection>> Execute(string query)
        {
            var wantedTracksYtIds = (await Resolve<YouTubeServices>().SearchIds(query)).ToArray();
            await Resolve<InsertTracksFromYouTubeVideosIfFound>().Execute(wantedTracksYtIds);
            var tracks = await GetTracks(wantedTracksYtIds);
            return tracks;
        }

        private async Task<IReadOnlyCollection<TrackForHomeSection>> GetTracks(IEnumerable<string> wantedTracksYtIds)
        {
            var curUserId = Resolve<ICurrentUserContext>().Id;
            var tracks = await Query<Track>()
                .Where(track => wantedTracksYtIds.Contains(track.YoutubeVideos.First().Id))
                .Select(TrackForHomeSection.FromTrack(curUserId))
                .ToArrayAsync();
            return tracks;
        }
    }
}
