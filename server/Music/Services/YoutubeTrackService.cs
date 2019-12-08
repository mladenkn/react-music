using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Utilities;

namespace Music.Services
{
    public class YoutubeTrackService
    {
        private readonly YouTubeService _youTubeService;

        public YoutubeTrackService(YouTubeService youTubeService)
        {
            _youTubeService = youTubeService;
        }

        public async Task<IEnumerable<Video>> GetList(IReadOnlyCollection<string> ids)
        {
            var r = new List<Video>(ids.Count);

            var chunkCount = ids.Count < 50 ? 1 : ids.Count / 50;

            foreach (var idsChunk in ids.Batch(chunkCount))
            {
                var allTracksFromYtRequest = _youTubeService.Videos.List("snippet,contentDetails");
                allTracksFromYtRequest.Id = string.Join(",", idsChunk);
                var allTracksFromYt = await allTracksFromYtRequest.ExecuteAsync();
                r.AddRange(allTracksFromYt.Items);
            }

            return r;
        }
    }
}
