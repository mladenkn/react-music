using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using Music.App.Models;

namespace Music.App.Services
{
    public partial class TracksService
    {
        public async Task<IEnumerable<string>> LearnYtVideos(string query)
        {
            var wantedTracksYtIds = (await Resolve<YouTubeVideosRemoteService>().SearchIds(query)).ToArray();
            await InsertFromYouTubeVideosIfFound(wantedTracksYtIds);
            return wantedTracksYtIds;
        }

        public async Task<IReadOnlyCollection<TrackForHomeSection>> GetTracksWithVideoIds(IEnumerable<string> wantedTracksYtIds)
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
