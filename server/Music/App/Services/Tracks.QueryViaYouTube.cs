using Music.App.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music.App.Services
{
    public partial class TracksService
    {
        public async Task<IEnumerable<TrackForHomeSection>> QueryViaYouTube(string searchQuery)
        {
            var foundVideosIds = await Resolve<YouTubeVideosRemoteService>().SearchIds(searchQuery);
            var newVideos = await Resolve<YouTubeVideosService>().EnsureAreSavedIfFound(foundVideosIds);

            var tracksService = Resolve<TracksService>();
            await tracksService.SaveTracksFromVideoIds(newVideos.Select(v => v.Id));
            return await tracksService.GetTracksWithVideoIdsIfFound(foundVideosIds);
        }
    }
}