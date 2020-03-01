using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music.App;
using Music.App.DbModels;
using Music.App.Models;

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
            var vids = await store.GetAll();
            var dbModels = await Map(vids);
            await Persist(ops =>
            {
                ops.InsertYouTubeChannels(dbModels);
                ops.InsertYouTubeVideos(vids.SelectMany(v => v.Videos), mutate: v => v.YouTubeChannel = null);
            });
        }

        private async Task<IEnumerable<YouTubeChannel>> Map(IEnumerable<YouTubeChannelWithVideos> channelsWithVids)
        {

        }
    }
}
