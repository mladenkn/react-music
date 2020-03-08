using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.Admin.Services;
using Music.App;
using Music.App.Services;

namespace Music.Admin.Tasks
{
    public class PersistAllChannelsVideosToFile : ServiceResolverAware
    {
        public PersistAllChannelsVideosToFile(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Execute()
        {
            var allChannels = await Db.YouTubeChannels.Skip(4).Take(1).ToArrayAsync();
            var ytService = Resolve<AdminYouTubeService>();
            var store = Resolve<ChannelVideosPersistantStore>();
            foreach (var youTubeChannel in allChannels)
            {
                try
                {
                    var channelWithVideos = await ytService.GetVideosOfChannel(youTubeChannel);
                    await store.Store(channelWithVideos);
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
