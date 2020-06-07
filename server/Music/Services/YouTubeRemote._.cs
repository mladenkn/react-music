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

        public async Task<IEnumerable<YouTubeChannelForAdmin>> GetChannelsOfUser(string username)
        {
            var ytService = Resolve<Google.Apis.YouTube.v3.YouTubeService>();
            var request = ytService.Channels.List("snippet,contentDetails");
            request.ForUsername = username;
            var response = await request.ExecuteAsync();
            var tasks = response.Items.Select(MapToYouTubeChannelDetails).ToArray();
            await Task.WhenAll(tasks);
            return tasks.Select(t => t.Result);
        }

        public async Task<YouTubeChannelForAdmin> GetChannelDetails(string channelId)
        {
            var ytService = Resolve<Google.Apis.YouTube.v3.YouTubeService>();
            var request = ytService.Channels.List("snippet,contentDetails");
            request.Id = channelId;
            var response = await request.ExecuteAsync();
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
            var videos = await Resolve<YouTubeRemoteService>().GetByIdsIfFound2(allVideosIds.ToArray(), videoParts);
            return videos;
        }

        private async Task<IReadOnlyList<string>> GetAllVideosIdsFromPlaylist(string playlistId, int? maxResults = null)
        {
            var ytService = Resolve<Google.Apis.YouTube.v3.YouTubeService>();
            var r = new List<string>();
            var remainingCount = maxResults;

            string nextPageToken = null;
            do
            {
                var request = ytService.PlaylistItems.List("contentDetails");
                request.PageToken = nextPageToken;
                request.PlaylistId = playlistId;
                request.MaxResults = remainingCount;
                var response = await request.ExecuteAsync();
                r.AddRange(response.Items.Select(i => i.ContentDetails.VideoId));
                nextPageToken = response.NextPageToken;
                remainingCount -= response.Items.Count;
            }
            while (nextPageToken != null && remainingCount != 0);

            return r;
        }
    }
}
