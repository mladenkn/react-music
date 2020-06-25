using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.EntityFrameworkCore;
using Music.DbModels;
using Music.Models;
using Utilities;

namespace Music.Services
{
    public class YouTubeRemoteService : ServiceResolverAware
    {
        public YouTubeRemoteService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<YouTubeChannelForAdmin>> GetChannelsOfUser(string username, bool ensureChannelsAreSaved = false)
        {
            var ytService = Resolve<YouTubeService>();
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
            var ytService = Resolve<YouTubeService>();
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
            var ytService = Resolve<YouTubeService>();
            var request = ytService.PlaylistItems.List("id");
            request.PlaylistId = playlistId;
            var response = await request.ExecuteAsync();
            return response.PageInfo.TotalResults ?? 0;
        }

        public async Task<IReadOnlyList<Video>> GetVideosOfChannel(string channelId, IReadOnlyCollection<string> videoParts, int maxResults, bool includeKnown = true)
        {
            var channel = await Query<YouTubeChannel>().FirstOrDefaultAsync(c => c.Id == channelId);
            var videosIds = await GetAllVideosIdsFromPlaylist(channel.UploadsPlaylistId, maxResults).Then(r => r.ToArray());
            if (!includeKnown)
                videosIds = await Resolve<YouTubeVideosService>().FilterToUnknownVideosIds(videosIds).Then(r => r.ToArray());
            var videos = await GetByIdsIfFound(videosIds, videoParts);
            return videos;
        }

        private async Task<IEnumerable<string>> GetAllVideosIdsFromPlaylist(string playlistId, int maxResults)
        {
            var ytService = Resolve<YouTubeService>();

            var request = ytService.PlaylistItems.List("contentDetails");
            request.PlaylistId = playlistId;
            request.MaxResults = maxResults;

            var r = await request.ExecuteWithPagingAsync();
            return r.Select(i => i.ContentDetails.VideoId);
        }


        public async Task<IReadOnlyList<Video>> GetByIdsIfFound(IReadOnlyCollection<string> ids, IReadOnlyCollection<string> videoParts)
        {
            var videosFromYt = new List<Video>(ids.Count);
            foreach (var idsChunk in ids.Batch(50))
            {
                var idsAsOneString = string.Join(",", idsChunk);
                var allVideosFromYt = await GetBase(req => req.Id = idsAsOneString, videoParts);
                videosFromYt.AddRange(allVideosFromYt);
            }
            return videosFromYt;
        }

        private async Task<List<Video>> GetBase(Action<VideosResource.ListRequest> consumeRequest, IEnumerable<string> videoParts)
        {
            var ytService = Resolve<YouTubeService>();
            var partsAsOneString = string.Join(",", videoParts);
            var request = ytService.Videos.List(partsAsOneString);
            consumeRequest(request);
            var result = await request.ExecuteAsync();
            return result.Items.ToList();
        }

        public async Task FetchChannelsPlaylistInfo(IReadOnlyCollection<YouTubeChannel> channels)
        {
            if (channels.Count == 0)
                return;

            var ytService = Resolve<YouTubeService>();
            var request = ytService.Channels.List("contentDetails");
            request.Id = string.Join(",", channels.Select(c => c.Id));
            var response = await request.ExecuteAsync();

            if (response.Items.Count != channels.Count)
            {
                var channelsIdsNotFetched = channels.Select(c => c.Id).Except(response.Items.Select(c => c.Id));
                var channelsIdsNotFetchedString = string.Join(",", channelsIdsNotFetched);
                throw new Exception($"Could not fetch channels with following ids: {channelsIdsNotFetchedString}");
            }

            foreach (var channel in channels)
            {
                var channelFromApi = response.Items.Single(c => c.Id == channel.Id);
                var channelPlaylists = channelFromApi.ContentDetails.RelatedPlaylists;
                channel.FavoritesPlaylistId = channelPlaylists.Favorites;
                channel.LikesPlaylistId = channelPlaylists.Likes;
                channel.UploadsPlaylistId = channelPlaylists.Uploads;
                channel.WatchHistoryPlaylistId = channelPlaylists.WatchHistory;
                channel.WatchLaterPlaylistId = channelPlaylists.WatchLater;
            }
        }

        public async Task<IEnumerable<string>> SearchIds(string query)
        {
            var httpClient = Resolve<HttpClient>();
            var htmlParser = Resolve<IBrowsingContext>();

            var r = await httpClient.GetAsync("https://www.youtube.com/results?search_query=" + query);
            var htmlString = await r.Content.ReadAsStringAsync();

            var document = await htmlParser.OpenAsync(c => c.Content(htmlString));

            var beforeIdUrlContent = "/watch?v=";

            var ids = document.QuerySelectorAll("#results a")
                .Where(anchorTag => anchorTag.Attributes.Any(a => a.Name == "href"))
                .Select(anchorTag => anchorTag.Attributes.First(a => a.Name == "href").Value)
                .Where(url => url.StartsWith(beforeIdUrlContent))
                .Select(url => url.Substring(beforeIdUrlContent.Length))
                .Distinct();

            return ids;
        }
    }
}
