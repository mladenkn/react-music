using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;
using Music.App;
using Music.DevOps.Models;

namespace Music.DevOps.Services
{
    public class DevOpsYouTubeService : ServiceResolverAware
    {
        public DevOpsYouTubeService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<YouTubeChannelDetails>> GetChannelsOfUser(string username)
        {
            var ytService = Resolve<Google.Apis.YouTube.v3.YouTubeService>();
            var request = ytService.Channels.List("snippet,contentDetails");
            request.ForUsername = username;
            var response = await request.ExecuteAsync();
            var tasks = response.Items.Select(MapToYouTubeChannelDetails).ToArray();
            await Task.WhenAll(tasks);
            return tasks.Select(t => t.Result);
        }

        public async Task<YouTubeChannelDetails> GetChannelDetails(string channelId)
        {
            var ytService = Resolve<Google.Apis.YouTube.v3.YouTubeService>();
            var request = ytService.Channels.List("snippet,contentDetails");
            request.Id = channelId;
            var response = await request.ExecuteAsync();
            var channel = response.Items.Single();
            return await MapToYouTubeChannelDetails(channel);
        }

        public async Task<YouTubeChannelDetails> MapToYouTubeChannelDetails(Channel channel)
        {
            var videosCount = await GetPlaylistVideoCount(channel.ContentDetails.RelatedPlaylists.Uploads);
            return new YouTubeChannelDetails
            {
                Id = channel.Id,
                Title = channel.Snippet.Title,
                VideosCount = videosCount
            };
        }

        private async Task<int> GetPlaylistVideoCount(string playlistId)
        {
            var ytService = Resolve<Google.Apis.YouTube.v3.YouTubeService>();
            var request = ytService.PlaylistItems.List("");
            request.PlaylistId = playlistId;
            var response = await request.ExecuteAsync();
            return response.PageInfo.TotalResults ?? 0;
        }
    }
}
