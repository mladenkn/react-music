using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Music.App;
using Music.App.DbModels;
using Music.App.Models;
using Music.App.Services;
using Utilities;

namespace Music.DevUtils
{
    public class PersistChannelsFromFileToDb : ServiceResolverAware
    {
        public PersistChannelsFromFileToDb(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Execute()
        {
            var store = Resolve<ChannelVideosPersistantStore>();
            var services = Resolve<SharedServices>();
            
            var channelsFromFile = await store.GetAll();
            var channels = await Map(channelsFromFile);
            var channelsNotInDb = await services.FilterToNotPersistedChannels(channels);
            var videos = await services.FilterToUnknownVideos(channelsFromFile.SelectMany(v => v.Videos).DistinctBy(v => v.Id));

            await Persist(ops =>
            {
                ops.InsertYouTubeChannels(channelsNotInDb);
                ops.InsertYouTubeVideos(videos, v => v.YouTubeChannel = null);
            });
        }

        private async Task<IEnumerable<YouTubeChannel>> Map(IEnumerable<YouTubeChannelWithVideos> channelsWithVids)
        {
            var channels = channelsWithVids
                .Select(YouTubeChannel.FromYouTubeChannelWithVideos)
                .DistinctBy(c => c.Id)
                .ToArray();
            var ytService = Resolve<YouTubeServices>();
            await ytService.FetchChannelsPlaylistInfo(channels);
            return channels;
        }
    }
}
