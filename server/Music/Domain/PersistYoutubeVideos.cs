using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.Domain.Shared;
using Music.Domain.YouTubeVideos;

namespace Music.Domain
{
    public class PersistYouTubeVideosIfFoundExecutor : ServiceResolverAware
    {
        public PersistYouTubeVideosIfFoundExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<string>> Execute(IEnumerable<string> wantedVideosIds)
        {
            var unknownVideosIds = (await FilterToUnknownVideosIds(wantedVideosIds)).ToArray();
            var videosFromYt = await Resolve<YouTubeVideoService>().GetByIds(unknownVideosIds);

            var dataPersistor = Resolve<DataPersistor>();
            await dataPersistor.InsertYoutubeVideos(videosFromYt);

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
    }
}
