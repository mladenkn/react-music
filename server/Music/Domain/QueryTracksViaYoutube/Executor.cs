using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AutoMapper.QueryableExtensions;
using Google.Apis.YouTube.v3;
using Kernel;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Models;
using Music.Domain.Shared;
using Utilities;

namespace Music.Domain.QueryTracksViaYoutube
{
    public class QueryTracksViaYoutubeExecutor : ServiceResolverAware
    {
        public QueryTracksViaYoutubeExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<TrackModel>> Execute(string searchQuery)
        {
            var wantedTracksIds = (await SearchVideoIdsOnYt(searchQuery)).ToArray();

            var notFoundVideosIds = await FilterToUnknownVideosIds(wantedTracksIds);
            var videosFromYt = await GetVideosFromYoutube(notFoundVideosIds.ToArray());
            Db.AddRange(videosFromYt);
            await Db.SaveChangesAsync();

            var tracks = await GetTracks(wantedTracksIds);

            return tracks;
        }

        private async Task<IEnumerable<string>> SearchVideoIdsOnYt(string searchQuery)
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

        private async Task<IReadOnlyCollection<string>> FilterToUnknownVideosIds(IEnumerable<string> ids)
        {
            var notFoundIds = await Db.Query<YoutubeVideo>()
                .Where(v => ids.All(id => id != v.Id))
                .Select(v => v.Id)
                .ToArrayAsync();
            return notFoundIds;
        }

        private async Task<IReadOnlyCollection<YoutubeVideoModel>> GetVideosFromYoutube(IReadOnlyCollection<string> ids)
        {
            var r = new List<YoutubeVideoModel>(ids.Count);
            var chunkCount = ids.Count < 50 ? 1 : ids.Count / 50;
            var youTubeService = Resolve<IYoutubeService>();

            foreach (var idsChunk in ids.Batch(chunkCount))
            {
                var allTracksFromYtRequest = youTubeService.ListVideos("snippet,contentDetails,statistics,topicDetails");
                allTracksFromYtRequest.Id = string.Join(",", idsChunk);
                var allVideosFromYt = await allTracksFromYtRequest.ExecuteAsync();
                var allVideosFromYtMapped = allVideosFromYt.Items.Select(v => Mapper.Map<YoutubeVideoModel>(v));
                r.AddRange(allVideosFromYtMapped);
            }

            return r;
        }

        private async Task<IReadOnlyCollection<TrackModel>> GetTracks(IEnumerable<string> ids)
        {
            // popravit
            var tracks = await Db.Query<DataAccess.Models.TrackUserProps>()
                .Where(t => ids.Contains(t.YoutubeVideoId))
                .ProjectTo<TrackModel>(Mapper.ConfigurationProvider)
                .ToListAsync();
            return tracks;
        }
    }
}
