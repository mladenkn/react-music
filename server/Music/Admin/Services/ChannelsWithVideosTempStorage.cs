using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Music.App.DbModels;
using Music.App.Models;
using Music.App.Services;
using Utilities;

namespace Music.Admin.Services
{
    public class ChannelsWithVideosTempStorage : ServiceResolverAware
    {
        public ChannelsWithVideosTempStorage(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        
        public async Task ToTemp(string channelId)
        {
            var channelWithVideos = await Resolve<AdminYouTubeService>().GetYouTubeChannelWithVideos(channelId);
            await Resolve<ChannelVideosPersistantStore>().Store(channelWithVideos);
        }

        public async Task FromTempToDb(string fileName)
        {
            var store = Resolve<ChannelVideosPersistantStore>();
            var services = Resolve<SharedServices>();

            var channel = (await store.Get(f => Path.GetFileName(f) == fileName)).Single();
            var videos = await services.FilterToUnknownVideos(channel.Videos);
            var tracks = videos.Select(v => new Track { YoutubeVideos = new[] { v } });

            var channelDbModel = channel.Videos.First(v => v.YouTubeChannel.UploadsPlaylistId != null).YouTubeChannel;

            await Persist(ops =>
            {
                ops.InsertYouTubeChannels(new []{ channelDbModel });
                ops.InsertTracks(tracks, t => t.YoutubeVideos.ForEach(v => v.YouTubeChannel = null));
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
