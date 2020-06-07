using System;
using System.Linq.Expressions;
using Music.DbModels;

namespace Music.Models
{
    public class YouTubeChannelDetailsForAdmin
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public int VideosCount { get; set; }

        public static Expression<Func<YouTubeChannel, YouTubeChannelDetailsForAdmin>> Map => channel => new YouTubeChannelDetailsForAdmin
        {
            Id = channel.Id,
            Title = channel.Title,
            VideosCount = channel.Videos.Count
        };
    }
}
