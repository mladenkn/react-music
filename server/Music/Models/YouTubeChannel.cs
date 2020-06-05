using System.Collections.Generic;
using Music.DbModels;

namespace Music.Models
{
    public class YouTubeChannelWithVideos
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public IReadOnlyList<YoutubeVideo> Videos { get; set; }
    }
    public class YouTubeChannelDetails
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public int VideosCount { get; set; }
    }
}
