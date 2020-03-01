using System;
using System.ComponentModel.DataAnnotations;
using Kernel;
using Music.App.Models;

namespace Music.App.DbModels
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

        public static YouTubeChannel FromYouTubeChannelWithVideos(YouTubeChannelWithVideos c) => new YouTubeChannel
        {
            Id = c.Id,
            Title = c.Title,
        };
    }
}
