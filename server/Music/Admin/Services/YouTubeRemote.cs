using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;
using Music.Admin.Models;
using Music.App.DbModels;
using Music.App.Models;
using Music.App.Services;

namespace Music.Admin.Services
{
    public class YouTubeRemoteService : ServiceResolverAware
    {
        public YouTubeRemoteService(IServiceProvider serviceProvider) : base(serviceProvider)
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

        private async Task<YouTubeChannelDetails> MapToYouTubeChannelDetails(Channel channel)
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
            var request = ytService.PlaylistItems.List("id");
            request.PlaylistId = playlistId;
            var response = await request.ExecuteAsync();
            return response.PageInfo.TotalResults ?? 0;
        }

        public async Task<YouTubeChannelWithVideos> GetVideosOfChannel(YouTubeChannel channel)
        {
            var allVideosIds = await GetAllVideosIdsFromPlaylist(channel.UploadsPlaylistId);
            var videos = await Resolve<YouTubeVideosRemoteService>().GetByIdsIfFound(allVideosIds.ToArray());
            return new YouTubeChannelWithVideos
            {
                Id = channel.Id,
                Title = channel.Title,
                Videos = videos
            };
        }

        private async Task<IReadOnlyList<string>> GetAllVideosIdsFromPlaylist(string playlistId)
        {
            var ytService = Resolve<Google.Apis.YouTube.v3.YouTubeService>();
            var r = new List<string>();

            string nextPageToken = null;
            do
            {
                var request = ytService.PlaylistItems.List("contentDetails");
                request.PageToken = nextPageToken;
                request.PlaylistId = playlistId;
                request.MaxResults = 50;
                var response = await request.ExecuteAsync();
                r.AddRange(response.Items.Select(i => i.ContentDetails.VideoId));
                nextPageToken = response.NextPageToken;
            }
            while (nextPageToken != null);

            return r;
        }

        public async Task<YouTubeChannelWithVideos> GetYouTubeChannelWithVideos(string channelId)
        {
            var ytService = Resolve<Google.Apis.YouTube.v3.YouTubeService>();
            var request = ytService.Channels.List("snippet,contentDetails");
            request.Id = channelId;
            var response = await request.ExecuteAsync();
            var channel = response.Items.Single();
            var allVideosIds = await GetAllVideosIdsFromPlaylist(channel.ContentDetails.RelatedPlaylists.Uploads);
            var videos = await Resolve<YouTubeVideosRemoteService>().GetByIdsIfFound(allVideosIds.ToArray());
            return new YouTubeChannelWithVideos
            {
                Id = channelId,
                Title = channel.Snippet.Title,
                Videos = videos
            };
        }
    }
}
