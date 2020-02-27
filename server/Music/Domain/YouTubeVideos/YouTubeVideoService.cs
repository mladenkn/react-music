using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Music.Domain.Shared.Models;
using Utilities;

namespace Music.Domain.YouTubeVideos
{
    public class YouTubeVideoService : ServiceResolverAware
    {
        public YouTubeVideoService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<string>> SearchIds(string query)
        {
            var httpClient = Resolve<HttpClient>();
            var htmlParser = Resolve<IBrowsingContext>();

            var r = await httpClient.GetAsync("https://www.youtube.com/results?search_query=" + query);
            var htmlString = await r.Content.ReadAsStringAsync();

            var document = await htmlParser.OpenAsync(c => c.Content(htmlString));

            var beforeIdUrlContent = "/watch?v=";

            var urls = document.QuerySelectorAll("#results a")
                .Where(anchorTag => anchorTag.Attributes.Any(a => a.Name == "href"))
                .Select(anchorTag => anchorTag.Attributes.First(a => a.Name == "href").Value)
                .Where(url => url.StartsWith(beforeIdUrlContent))
                .Select(url => url.Substring(beforeIdUrlContent.Length));

            return urls;
        }

        public async Task<IEnumerable<YoutubeVideo>> GetByIds(IReadOnlyCollection<string> ids)
        {
            var videosFromYt = new List<Video>(ids.Count);

            foreach (var idsChunk in ids.Batch(50))
            {
                var allVideosFromYt = await GetBase(new[] { "snippet", "contentDetails", "statistics", "topicDetails" }, idsChunk);
                videosFromYt.AddRange(allVideosFromYt);
            }

            foreach (var videoFromYt in videosFromYt)
            {
                if (videoFromYt.ContentDetails == null)
                    throw new Exception("Video from YouTube API missing ContentDetails part");
                if (videoFromYt.Snippet == null)
                    throw new Exception("Video from YouTube API missing Snippet part");
                if (videoFromYt.Snippet.Thumbnails == null)
                    throw new Exception("Video from YouTube API missing Snippet.Thumbnails part");
                if (videoFromYt.Statistics == null)
                    throw new Exception("Video from YouTube API missing Snippet part");
            }

            var videosFromYtMapped = videosFromYt.Select(v => Mapper.Map<YoutubeVideo>(v));
            return videosFromYtMapped;
        }

        public async Task<List<Video>> GetBase(IEnumerable<string> parts, IEnumerable<string> ids = null)
        {
            var partsAsOneString = string.Join(",", parts);
            var idsAsOneString = string.Join(",", ids);
            var ytService = Resolve<YouTubeService>();
            var request = ytService.Videos.List(partsAsOneString);
            request.Id = idsAsOneString;
            var result = await request.ExecuteAsync();
            return result.Items.ToList();
        }
    }
}
