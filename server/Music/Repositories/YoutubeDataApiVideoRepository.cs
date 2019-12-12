using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Music.Models;
using Utilities;

namespace Music.Repositories
{
    public class YoutubeDataApiVideoRepository
    {
        private readonly YouTubeService _youTubeService;

        public YoutubeDataApiVideoRepository(YouTubeService youTubeService)
        {
            _youTubeService = youTubeService;
        }

        public async Task<IReadOnlyCollection<YoutubeVideo>> GetList(IReadOnlyCollection<string> ids)
        {
            var r = new List<YoutubeVideo>(ids.Count);

            var chunkCount = ids.Count < 50 ? 1 : ids.Count / 50;

            foreach (var idsChunk in ids.Batch(chunkCount))
            {
                var allTracksFromYtRequest = _youTubeService.Videos.List("snippet");
                allTracksFromYtRequest.Id = string.Join(",", idsChunk);
                var allTracksFromYt = await allTracksFromYtRequest.ExecuteAsync();
                r.AddRange(allTracksFromYt.Items.Select(MapToYoutubeVideo));
            }

            return r;
        }

        private static YoutubeVideo MapToYoutubeVideo(Video fromYt) => new YoutubeVideo
        {
            Id = fromYt.Id,
            Title = fromYt.Snippet.Title,
            Image = fromYt.Snippet.Thumbnails.Medium.Url,
            Description = fromYt.Snippet.Description,
            ChannelId = fromYt.Snippet.ChannelId,
            ChannelTitle = fromYt.Snippet.ChannelTitle,
        };
    }
}
