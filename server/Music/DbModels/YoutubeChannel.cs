using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Google.Apis.YouTube.v3.Data;
using Kernel;

namespace Music.DbModels
{
    public class YouTubeChannel : IDatabaseEntity
    {
        [Required] 
        public string Id { get; set; }

        [Required] 
        public string Title { get; set; }

        public string FavoritesPlaylistId { get; set; }

        public string UploadsPlaylistId { get; set; }

        public string LikesPlaylistId { get; set; }

        public string WatchLaterPlaylistId { get; set; }

        public string WatchHistoryPlaylistId { get; set; }

        public DateTime LastUpdateAt { get; set; }

        public IReadOnlyList<YoutubeVideo> Videos { get; set; }

        public static YouTubeChannel FromYouTubeApiChannel(Channel video)
        {
            var playlists = video.ContentDetails.RelatedPlaylists;
            return new YouTubeChannel
            {
                Id = video.Id,
                Title = video.Snippet.Title,
                FavoritesPlaylistId = playlists.Favorites,
                UploadsPlaylistId = playlists.Uploads,
                LikesPlaylistId = playlists.Likes,
                WatchLaterPlaylistId = playlists.WatchLater,
                WatchHistoryPlaylistId = playlists.WatchHistory,
            };
        }
    }
}
