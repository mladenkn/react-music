using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using Utilities;

namespace Music.App.Services
{
    public class SharedServices : ServiceResolverAware
    {
        public SharedServices(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<YouTubeChannel>> FilterToNotPersistedChannels(
            IEnumerable<YouTubeChannel> channels)
        {
            var allChannelsIdsFromDb = await Query<YouTubeChannel>().Select(c => c.Id).ToArrayAsync();
            var filtered = channels.Where(c => !c.Id.IsIn(allChannelsIdsFromDb));
            return filtered;
        }

        public async Task<IEnumerable<YoutubeVideo>> FilterToUnknownVideos(IEnumerable<YoutubeVideo> videos)
        {
            var allVideosIds = await Query<YoutubeVideo>().Select(v => v.Id).ToArrayAsync();
            var filtered = videos.Where(v => !v.Id.IsIn(allVideosIds));
            return filtered;
        }
    }
}
