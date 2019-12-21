using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Music.Models;

namespace Music.Repositories
{
    public class YoutubeHtmlScrapperVideoRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IBrowsingContext _htmlParser;

        public YoutubeHtmlScrapperVideoRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;

            var config = Configuration.Default;
            _htmlParser = BrowsingContext.New(config);
        }

        public async Task<IEnumerable<YoutubeVideo>> GetList(IReadOnlyCollection<string> ids)
        {
            foreach (var videoId in ids)
            {
                var videoHtmlString = await GetVideoHtml(videoId);

                //var title = videoHtml.DocumentNode.Descendants("h1")
                //    .SelectMany(x => x.Descendants())
                //    .Where(x => x.Attributes["class"].Value == "title style-scope ytd-video-primary-info-renderer");
                
                //var title = videoHtml.DocumentNode.SelectSingleNode("/html/body/ytd-app/div/ytd-page-manager/ytd-watch-flexy/div[4]/div[1]/div/div[5]/div[2]/ytd-video-primary-info-renderer/div/h1/yt-formatted-string");
                //var channel = videoHtml.DocumentNode.SelectSingleNode("/html/body/ytd-app/div/ytd-page-manager/ytd-watch-flexy/div[4]/div[1]/div/div[7]/div[3]/ytd-video-secondary-info-renderer/div/div[2]/ytd-video-owner-renderer/div[1]/ytd-channel-name/div/div/yt-formatted-string/a");

                var document = await _htmlParser.OpenAsync(req => req.Content(videoHtmlString));

                var titleAndDescriptionQuery = document.All
                    .Single(e => e.Id == "watch7-main-container")
                    .Descendents()
                    .Where(n => n.NodeType == NodeType.Element)
                    .Cast<IElement>();

                var title = titleAndDescriptionQuery
                   .Single(e => e.Attributes.Any(a => a.Name == "itemprop" && a.Value == "name") && e.LocalName == "meta")
                   .TextContent;

                var description = titleAndDescriptionQuery
                    .Single(e => e.Attributes.Any(a => a.Name == "itemprop" && a.Value == "description") && e.LocalName == "meta")
                    .TextContent;

                Console.Write("");
            }

            return null;
        }

        public async Task<IEnumerable<string>> SearchIds(string searchQuery)
        {
            var r = await _httpClient.GetAsync("https://www.youtube.com/results?search_query=" + searchQuery);
            var htmlString = await r.Content.ReadAsStringAsync();

            var document = await _htmlParser.OpenAsync(c => c.Content(htmlString));

            var beforeIdUrlContent = "/watch?v=";

            var urls = document.QuerySelectorAll("#results a")
                .Where(anchorTag => anchorTag.Attributes.Any(a => a.Name == "href"))
                .Select(anchorTag => anchorTag.Attributes.First(a => a.Name == "href").Value)
                .Where(url => url.StartsWith(beforeIdUrlContent))
                .Select(url => url.Substring(beforeIdUrlContent.Length));

            return urls;
        }

        private async Task<string> GetVideoHtml(string videoId)
        {
            //var r = await _httpClient.GetAsync("https://www.youtube.com/watch?v=" + videoId);
            var r = await _httpClient.GetAsync("https://www.youtube.com/watch?v=CNEpEA9OhR0");
            var htmlString = await r.Content.ReadAsStringAsync();
            return htmlString;
        }
    }
}
