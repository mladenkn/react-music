using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Music.DbModels;
using Utilities;

namespace Music.Services
{
    public partial class YouTubeRemoteService
    {

        private readonly string[] _videoParts = { "snippet", "contentDetails", "statistics", "topicDetails" };

        public async Task<IReadOnlyList<YoutubeVideo>> GetByIdsIfFound(IReadOnlyCollection<string> ids)
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

        public async Task<IReadOnlyList<Video>> GetByIdsIfFound2(IReadOnlyCollection<string> ids, IEnumerable<string> videoParts)
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
            await FetchChannelsPlaylistInfo(channels);
            return videosFromYtMapped;
        }

        private async Task<List<Video>> GetBase(Action<VideosResource.ListRequest> consumeRequest, IEnumerable<string> videoParts = null)
        {
            var ytService = Resolve<YouTubeService>();
            var partsAsOneString = string.Join(",", videoParts ?? _videoParts);
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
    }
}
