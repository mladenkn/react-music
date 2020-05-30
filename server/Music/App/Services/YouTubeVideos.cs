using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

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
            var channelsToInsert = await FilterToNotPersistedChannels(
                videos.Select(v => v.YouTubeChannel).DistinctBy(c => c.Id)
            );

            await Persist(ops =>
            {
                channelsToInsert.ForEach(ops.Add);
                videos.ForEach(vid =>
                {
                    ops.Add(
                        vid, 
                        v => v.YouTubeChannel = null, 
                        (original, copy) => original.Id = copy.Id
                    );
                });
            });
        }

        private async Task<IEnumerable<YouTubeChannel>> FilterToNotPersistedChannels(
            IEnumerable<YouTubeChannel> channels)
        {
            var allChannelsIdsFromDb = await Query<YouTubeChannel>().Select(c => c.Id).ToArrayAsync();
            var filtered = channels.Where(c => !c.Id.IsIn(allChannelsIdsFromDb));
            return filtered;
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
