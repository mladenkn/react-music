using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App;
using Music.App.YouTubeVideos;

namespace Music.DevUtils
{
    public class PersistAllChannelsVideosToFile : ServiceResolverAware
    {
        public PersistAllChannelsVideosToFile(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Execute()
        {
            var allChannels = await Db.YouTubeChannels.ToArrayAsync();
            var ytService = Resolve<YouTubeVideoService>();
            foreach (var youTubeChannel in allChannels)
            {
                try
                {
                    var channelWithVideos = await ytService.GetVideosOfChannel(youTubeChannel);
                    var store = Resolve<ChannelVideosPersistantStore>();
                    await store.Store(channelWithVideos);
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
