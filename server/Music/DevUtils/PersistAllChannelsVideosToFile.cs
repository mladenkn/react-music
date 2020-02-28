using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App;
using Music.App.YouTubeVideos;
using Newtonsoft.Json;

namespace Music.DevUtils
{
    public class PersistAllChannelsVideosToFile : ServiceResolverAware
    {
        public PersistAllChannelsVideosToFile(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Execute(string folder)
        {
            var allChannels = await Db.YouTubeChannels.ToArrayAsync();
            var ytService = Resolve<YouTubeVideoService>();
            foreach (var youTubeChannel in allChannels)
            {
                var doesExist = await ytService.DoesChannelExist(youTubeChannel.Id);
                if(!doesExist)
                    throw new Exception();
            }
            var channelsWithVideos = await ytService.GetVideosOfChannels(allChannels);
            var totalVideoCount = channelsWithVideos.Sum(c => c.Videos.Count);

            foreach (var channelWithVids in channelsWithVideos)
            {
                var filePath = Path.Combine(folder, $"{channelWithVids.Title} - {channelWithVids.Id}");
                var channelJson = JsonConvert.SerializeObject(channelWithVids, Formatting.Indented);
                await using var writer = File.CreateText(filePath);
                await writer.WriteAsync(channelJson);
            }
        }
    }
}
