using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;
using Microsoft.EntityFrameworkCore;
using Music.DbModels;
using Music.Models;

namespace Music.Services
{
    public partial class YouTubeRemoteService
    {

        public async Task<IEnumerable<YouTubeChannelForAdmin>> GetChannelsOfUser(string username, bool ensureChannelsAreSaved = false)
        {
            var ytService = Resolve<Google.Apis.YouTube.v3.YouTubeService>();
            var request = ytService.Channels.List("snippet,contentDetails");
            request.ForUsername = username;
            var response = await request.ExecuteAsync();
            if(response.Items == null || !response.Items.Any())
                throw new ApplicationException("Channel not found");
            var tasks = response.Items.Select(MapToYouTubeChannelDetails).ToArray();
            await Task.WhenAll(tasks);
            if (ensureChannelsAreSaved)
                await Resolve<YouTubeChannelService>().EnsureAreSaved(response.Items);
            return tasks.Select(t => t.Result);
        }

        public async Task<YouTubeChannelForAdmin> GetChannelDetails(string channelId, bool saveChannel = false)
        {
            var ytService = Resolve<Google.Apis.YouTube.v3.YouTubeService>();
            var request = ytService.Channels.List("snippet,contentDetails");
            request.Id = channelId;
            var response = await request.ExecuteAsync();
            if (saveChannel)
                await Resolve<YouTubeChannelService>().EnsureAreSaved(response.Items);
            var channel = response.Items.Single();
            return await MapToYouTubeChannelDetails(channel);
        }

        private async Task<YouTubeChannelForAdmin> MapToYouTubeChannelDetails(Channel channel)
        {
            var videosCount = await GetPlaylistVideoCount(channel.ContentDetails.RelatedPlaylists.Uploads);
            return new YouTubeChannelForAdmin
            {
                Id = channel.Id,
                Title = channel.Snippet.Title,
                VideosCount = videosCount
            };
        }

        private async Task<int> GetPlaylistVideoCount(string playlistId)
        {
            var ytService = Resolve<Google.Apis.YouTube.v3.YouTubeService>();
            var request = ytService.PlaylistItems.List("id");
            request.PlaylistId = playlistId;
            var response = await request.ExecuteAsync();
            return response.PageInfo.TotalResults ?? 0;
        }

        public async Task<IReadOnlyList<Video>> GetVideosOfChannel(string channelId, IEnumerable<string> videoParts, int? maxResults)
        {
            var channel = await Query<YouTubeChannel>().FirstOrDefaultAsync(c => c.Id == channelId);
            var allVideosIds = await GetAllVideosIdsFromPlaylist(channel.UploadsPlaylistId, maxResults);
            var videos = await GetByIdsIfFound2(allVideosIds.ToArray(), videoParts);
            return videos;
        }

        private async Task<IEnumerable<string>> GetAllVideosIdsFromPlaylist(string playlistId, int? maxResults = null)
        {
            var ytService = Resolve<Google.Apis.YouTube.v3.YouTubeService>();
            var remainingCount = maxResults;

            var request = ytService.PlaylistItems.List("contentDetails");
            request.PlaylistId = playlistId;
            request.MaxResults = remainingCount;

            var r = await request.ExecuteWithPagingAsync();
            return r.Select(i => i.ContentDetails.VideoId);
        }
    }
}
