using System.Collections.Generic;
using Music.App.DbModels;

namespace Music.App.Models
{
    public class YouTubeChannelWithVideos
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public IReadOnlyList<YoutubeVideo> Videos { get; set; }
    }
}
