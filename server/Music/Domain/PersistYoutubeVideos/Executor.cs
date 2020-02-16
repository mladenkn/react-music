using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess;
using Music.DataAccess.Models;
using Music.Domain.QueryTracksViaYoutube;
using Utilities;

namespace Music.Domain.PersistYoutubeVideos
{
    public class PersistYouTubeVideosIfFoundExecutor : ServiceResolverAware
    {
        public PersistYouTubeVideosIfFoundExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<string>> Execute(IEnumerable<string> wantedVideosIds)
        {
            var unknownVideosIds = await FilterToUnknownVideosIds(wantedVideosIds);
            var videosFromYt = await GetVideosFromYoutube(unknownVideosIds.ToArray());

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

            var dataPersistor = Resolve<DataPersistor>();
            await dataPersistor.InsertYoutubeVideos(videosFromYtMapped);

            var notFoundVideosIds = wantedVideosIds.Except(videosFromYt.Select(v => v.Id));
            return notFoundVideosIds;
        }

        private async Task<IEnumerable<string>> FilterToUnknownVideosIds(IEnumerable<string> ids)
        {
            var foundIds = await Db.YoutubeVideos
                .Select(v => v.Id)
                .Where(vId => ids.Contains(vId))
                .ToArrayAsync();
            var notFoundIds = ids.Except(foundIds);
            return notFoundIds;
        }

        private async Task<IReadOnlyCollection<Video>> GetVideosFromYoutube(IReadOnlyCollection<string> ids)
        {
            var r = new List<Video>(ids.Count);
            var listVideos = Resolve<ListYoutubeVideos>();

            foreach (var idsChunk in ids.Batch(50))
            {
                var allVideosFromYt = await listVideos(new[] { "snippet", "contentDetails", "statistics", "topicDetails" }, idsChunk);
                r.AddRange(allVideosFromYt);
            }

            return r;
        }
    }
}
