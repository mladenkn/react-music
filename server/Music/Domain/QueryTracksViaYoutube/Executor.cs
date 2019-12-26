using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AutoMapper.QueryableExtensions;
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
            var searchYoutubeVideosIds = Resolve<SearchYoutubeVideosIds>();
            var wantedTracksIds = (await searchYoutubeVideosIds(searchQuery)).ToArray();

            var notFoundVideosIds = await FilterToUnknownVideosIds(wantedTracksIds);
            var videosFromYt = await GetVideosFromYoutube(notFoundVideosIds.ToArray());
            Db.AddRange(videosFromYt);
            await Db.SaveChangesAsync();

            var tracks = await GetTracks(wantedTracksIds);

            return tracks;
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
            var listVideos = Resolve<ListYoutubeVideos>();

            foreach (var idsChunk in ids.Batch(chunkCount))
            {
                var allVideosFromYt = await listVideos(new [] {"snippet","contentDetails","statistics","topicDetails"}, idsChunk);
                var allVideosFromYtMapped = allVideosFromYt.Select(v => Mapper.Map<YoutubeVideoModel>(v));
                r.AddRange(allVideosFromYtMapped);
            }

            return r;
        }

        private async Task<IReadOnlyCollection<TrackModel>> GetTracks(IEnumerable<string> wantedTracksIds)
        {
            var tracks = await Db.Query<YoutubeVideo>()
                .Where(t => wantedTracksIds.Contains(t.Id))
                .ProjectTo<TrackModel>(Mapper.ConfigurationProvider)
                .ToListAsync();
            return tracks;
        }
    }
}
