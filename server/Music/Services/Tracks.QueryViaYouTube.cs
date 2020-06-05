using System.Collections.Generic;
using System.Threading.Tasks;
using Music.Models;

namespace Music.Services
{
    public partial class TracksService
    {
        public async Task<IEnumerable<TrackForHomeSection>> QueryViaYouTube(string searchQuery)
        {
            var foundVideosIds = await Resolve<YouTubeRemoteService>().SearchIds(searchQuery);
            var newVideos = await Resolve<YouTubeVideosService>().EnsureAreSavedIfFound(foundVideosIds);

            var tracksService = Resolve<TracksService>();
            await tracksService.SaveTracksFromVideos(newVideos);
            return await tracksService.GetTracksWithVideoIdsIfFound(foundVideosIds);
        }
    }
}