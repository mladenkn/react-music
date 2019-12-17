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
        public async Task<IReadOnlyCollection<YoutubeVideo>> Search(YoutubeTrackQuery query)
        {
            var request = _youTubeService.Search.List("snippet");
            request.Type = "video";
            request.Q = query.SearchQuery;
            request.MaxResults = query.MaxResults;

            var r = await request.ExecuteAsync();
            var videos = r.Items.Select(item => new YoutubeVideo
            {
                ChannelId = item.Snippet.ChannelId,
                ChannelTitle = item.Snippet.ChannelTitle,
                Description = item.Snippet.Description,
                Id = item.Id.VideoId,
                Image = item.Snippet.Thumbnails.Medium.Url,
                Title = item.Snippet.Title,
            }).ToArray();

            return videos;
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

    public class YoutubeTrackQuery
    {
        public string SearchQuery { get; set; }
        public string ChannelTitle { get; set; }
        public int MaxResults { get; set; } 
    }
}
