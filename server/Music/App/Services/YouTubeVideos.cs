using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music.App.Services
{
    public class YouTubeVideosService : ServiceResolverAware
    {
        public YouTubeVideosService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IReadOnlyList<YoutubeVideo>> EnsureAreSavedIfFound(IEnumerable<string> possibleIds)
        {
            var newVideosIds = (await FilterToUnknownVideosIds(possibleIds)).ToArray();
            var newVideos = await Resolve<YouTubeVideosRemoteService>().GetByIdsIfFound(newVideosIds);
            await Save(newVideos);
            return newVideos;
        }

        public async Task Save(IEnumerable<YoutubeVideo> videos)
        {

        }

        public async Task<IEnumerable<string>> FilterToUnknownVideosIds(IEnumerable<string> ids)
        {
            var foundIds = await Query<YoutubeVideo>()
                .Select(v => v.Id)
                .Where(vId => ids.Contains(vId))
                .ToArrayAsync();
            var notFoundIds = ids.Except(foundIds);
            return notFoundIds;
        }
    }
}
