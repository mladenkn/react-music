using System;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Music.App;
using Music.App.Services;

namespace Music.DevOps.Tasks
{
    public class PersistAllChannelsVideosToFile : ServiceResolverAware
    {
        public PersistAllChannelsVideosToFile(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Execute()
        {
            var allChannels = await Db.YouTubeChannels.Skip(4).Take(1).ToArrayAsync();
            var ytService = Resolve<YouTubeServices>();
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

        public static void ConfigureCommand(CommandLineApplication c, IServiceProvider sp)
        {
            c.Command("channels-to-file", cmd =>
            {
                cmd.OnExecuteAsync(async _ =>
                {
                    await new PersistAllChannelsVideosToFile(sp).Execute();
                });
            });
        }
    }
}
