using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;

namespace Music.App.Services
{
    public partial class YouTubeVideosRemoteService : ServiceResolverAware
    {
        public YouTubeVideosRemoteService(IServiceProvider serviceProvider) : base(serviceProvider)
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

            var ids = document.QuerySelectorAll("#results a")
                .Where(anchorTag => anchorTag.Attributes.Any(a => a.Name == "href"))
                .Select(anchorTag => anchorTag.Attributes.First(a => a.Name == "href").Value)
                .Where(url => url.StartsWith(beforeIdUrlContent))
                .Select(url => url.Substring(beforeIdUrlContent.Length))
                .Distinct();

            return ids;
        }
    }
}
