using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using Google.Apis.YouTube.v3;
using Kernel;

namespace Music.Domain.QueryTracksViaYoutube
{
    public delegate VideosResource.ListRequest ListYoutubeVideos(string part);
    public delegate Task<IEnumerable<string>> SearchYoutubeVideosIds(string searchQuery);

    public class QueryTracksViaYoutubeServices : ServiceResolverAware
    {
        public QueryTracksViaYoutubeServices(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<string>> SearchYoutubeVideosIds(string searchQuery)
        {
            var httpClient = Resolve<HttpClient>();
            var htmlParser = Resolve<IBrowsingContext>();

            var r = await httpClient.GetAsync("https://www.youtube.com/results?search_query=" + searchQuery);
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
    }
}
