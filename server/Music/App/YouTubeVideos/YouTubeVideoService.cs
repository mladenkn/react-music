using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Music.App.DbModels;
using Music.App.Models;
using Utilities;

namespace Music.App.YouTubeVideos
{
    public class YouTubeVideoService : ServiceResolverAware
    {
        public YouTubeVideoService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        private readonly string[] _videoParts = { "snippet", "contentDetails", "statistics", "topicDetails" };

        public async Task<IEnumerable<string>> SearchIds(string query)
        {
            var httpClient = Resolve<HttpClient>();
            var htmlParser = Resolve<IBrowsingContext>();

            var r = await httpClient.GetAsync("https://www.youtube.com/results?search_query=" + query);
            var htmlString = await r.Content.ReadAsStringAsync();

            var document = await htmlParser.OpenAsync(c => c.Content(htmlString));

            var beforeIdUrlContent = "/watch?v=";

            var urls = document.QuerySelectorAll("#results a")
                .Where(anchorTag => anchorTag.Attributes.Any(a => a.Name == "href"))
                .Select(anchorTag => anchorTag.Attributes.First(a => a.Name == "href").Value)
                .Where(url => url.StartsWith(beforeIdUrlContent))
                .Select(url => url.Substring(beforeIdUrlContent.Length));

            return urls;
        }

        public async Task<bool> DoesChannelExist(string channelId)
        {
            var service = Resolve<YouTubeService>();
            var req = service.Channels.List("id");
            req.Id = channelId;
            var result = await req.ExecuteAsync();
            return result.Items.Single().Id == channelId;
        }

        public async Task<IReadOnlyList<YoutubeVideo>> GetByIds(IReadOnlyCollection<string> ids)
        {
            var videosFromYt = new List<Video>(ids.Count);

            foreach (var idsChunk in ids.Batch(50))
            {
                var idsAsOneString = string.Join(",", idsChunk);
                var allVideosFromYt = await GetBase(req => req.Id = idsAsOneString);
                videosFromYt.AddRange(allVideosFromYt);
            }

            return await PostRead(videosFromYt.ToArray());
        }

        public async Task<IReadOnlyList<YouTubeChannelWithVideos>> GetVideosOfChannels(IReadOnlyCollection<YouTubeChannel> channels)
        {
            var allVideosIdsByChannel = await GetAllVideosIdsOfChannels(channels);

            var videosByChannels = new Dictionary<string, Task<IReadOnlyList<YoutubeVideo>>>();
            
            foreach (var keyValuePair in allVideosIdsByChannel)
            {
                var (channelId, videosIds) = keyValuePair;
                var videosTask = GetByIds(videosIds.ToArray());
                videosByChannels[channelId] = videosTask;
            }
            await Task.WhenAll(videosByChannels.Values);
            var result = videosByChannels
                .Aggregate(new Dictionary<string, IEnumerable<YoutubeVideo>>(),
                (accumulate, item) =>
                {
                    accumulate[item.Key] = item.Value.Result;
                    return accumulate;
                })
                .Select(e =>
                {
                    var channel = channels.Single(c => c.Id == e.Key);
                    return new YouTubeChannelWithVideos
                    {
                        Id = channel.Id,
                        Title = channel.Title,
                        Videos = e.Value.ToArray()
                    };
                })
                .ToArray();
            return result;
        }

        public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetAllVideosIdsOfChannels(IEnumerable<YouTubeChannel> channels)
        {
            var videoIdsByChannelsTasks = new Dictionary<string, Task<IReadOnlyList<string>>>();
            foreach (var channel in channels)
            {
                var task = GetAllVideosIdsFromPlaylist(channel.UploadsPlaylistId);
                videoIdsByChannelsTasks[channel.Id] = task;
            }
            await Task.WhenAll(videoIdsByChannelsTasks.Values);
            var r = videoIdsByChannelsTasks.Aggregate(new Dictionary<string, IReadOnlyList<string>>(),
                (accumulate, item) =>
                {
                    accumulate[item.Key] = item.Value.Result;
                    return accumulate;
                });
            return r;
        }

        private async Task<IReadOnlyList<string>> GetAllVideosIdsFromPlaylist(string playlistId)
        {
            var ytService = Resolve<YouTubeService>();
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

        private async Task<YoutubeVideo[]> PostRead(IReadOnlyCollection<Video> vids)
        {
            foreach (var videoFromYt in vids)
            {
                if (videoFromYt.ContentDetails == null)
                    throw new Exception("Video from YouTube API missing ContentDetails part");
                if (videoFromYt.Snippet == null)
                    throw new Exception("Video from YouTube API missing Snippet part");
                if (videoFromYt.Snippet.Thumbnails == null)
                    throw new Exception("Video from YouTube API missing Snippet.Thumbnails part");
                if (videoFromYt.Statistics == null)
                    throw new Exception("Video from YouTube API missing Snippet part");
            }
            
            var videosFromYtMapped = vids.Select(v => Mapper.Map<YoutubeVideo>(v)).ToArray();
            var channels = videosFromYtMapped.Select(v => v.YouTubeChannel).DistinctBy(c => c.Id).ToArray();
            await FetchChannelsAdditionalData(channels);
            return videosFromYtMapped;
        }

        private async Task<List<Video>> GetBase(Action<VideosResource.ListRequest> consumeRequest)
        {
            var ytService = Resolve<YouTubeService>();
            var partsAsOneString = string.Join(",", _videoParts);
            var request = ytService.Videos.List(partsAsOneString);
            consumeRequest(request);
            var result = await request.ExecuteAsync();
            return result.Items.ToList();
        }

        private async Task FetchChannelsAdditionalData(IReadOnlyCollection<YouTubeChannel> channels)
        {
            var ytService = Resolve<YouTubeService>();
            var request = ytService.Channels.List("contentDetails");
            request.Id = string.Join(",", channels.Select(c => c.Id));
            var response = await request.ExecuteAsync();

            if(response.Items.Count != channels.Count)
                throw new Exception();
            
            foreach (var channel in channels)
            {
                var channelFromApi = response.Items.Single(c => c.Id == channel.Id);
                var channelPlaylists = channelFromApi.ContentDetails.RelatedPlaylists;
                channel.FavoritesPlaylistId = channelPlaylists.Favorites;
                channel.LikesPlaylistId = channelPlaylists.Likes;
                channel.UploadsPlaylistId= channelPlaylists.Uploads;
                channel.WatchHistoryPlaylistId = channelPlaylists.WatchHistory;
                channel.WatchLaterPlaylistId = channelPlaylists.WatchLater;
            }
        }
    }
}
