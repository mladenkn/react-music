using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using Music.App.Models;
using Utilities;

namespace Music.App.Services
{
    public partial class TracksService
    {
        public async Task<IReadOnlyCollection<TrackForHomeSection>> GetTracksWithVideoIdsIfFound(IEnumerable<string> wantedTracksYtIds)
        {
            var curUserId = Resolve<ICurrentUserContext>().Id;
            var tracks = await Query<Track>()
                .Where(track => wantedTracksYtIds.Contains(track.YoutubeVideos.First().Id))
                .Select(TrackForHomeSection.FromTrack(curUserId))
                .ToArrayAsync();
            return tracks;
        }

        public async Task<IEnumerable<Track>> SaveTracksFromVideoIds(IEnumerable<string> videoIds)
        {

        }

        public async Task<IEnumerable<YouTubeChannel>> FilterToNotPersistedChannels(
            IEnumerable<YouTubeChannel> channels)
        {
            var allChannelsIdsFromDb = await Query<YouTubeChannel>().Select(c => c.Id).ToArrayAsync();
            var filtered = channels.Where(c => !c.Id.IsIn(allChannelsIdsFromDb));
            return filtered;
        }
    }
}
