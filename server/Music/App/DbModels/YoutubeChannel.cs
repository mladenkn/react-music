using System.ComponentModel.DataAnnotations;

namespace Music.App.Models
{
    public class YouTubeChannel
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
    }
}
