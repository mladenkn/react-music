using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            var channelWithVideos = await Resolve<YouTubeRemoteService>().GetYouTubeChannelWithVideos(channelId);
            await Resolve<ChannelVideosPersistantStore>().Store(channelWithVideos);
        }

        public async Task FromTempToDb(string fileName)
        {
            var store = Resolve<ChannelVideosPersistantStore>();

            var channel = (await store.Get(f => Path.GetFileName(f) == fileName)).Single();
            var videos = await FilterToUnknownVideos(channel.Videos);
            var tracks = videos.Select(v => new Track { YoutubeVideos = new[] { v } });

            var channelDbModel = channel.Videos.First(v => v.YouTubeChannel.UploadsPlaylistId != null).YouTubeChannel;

            await Persist(ops =>
            {
                ops.InsertYouTubeChannels(new []{ channelDbModel });
                ops.InsertTracks(tracks, t => t.YoutubeVideos.ForEach(v => v.YouTubeChannel = null));
            });
        }

        private async Task<IEnumerable<YoutubeVideo>> FilterToUnknownVideos(IEnumerable<YoutubeVideo> videos)
        {
            var allVideosIds = await Query<YoutubeVideo>().Select(v => v.Id).ToArrayAsync();
            var filtered = videos.Where(v => !v.Id.IsIn(allVideosIds));
            return filtered;
        }

        private async Task<IEnumerable<YouTubeChannel>> Map(IEnumerable<YouTubeChannelWithVideos> channelsWithVids)
        {
            var channels = channelsWithVids
                .Select(YouTubeChannel.FromYouTubeChannelWithVideos)
                .DistinctBy(c => c.Id)
                .ToArray();
            var ytService = Resolve<YouTubeVideosRemoteService>();
            await ytService.FetchChannelsPlaylistInfo(channels);
            return channels;
        }
    }
}
