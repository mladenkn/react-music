using System;
using System.Collections.Generic;

namespace Music.DataAccess.Models
{
    public class YoutubeVideoDbModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string YoutubeChannelId { get; set; }

        public YoutubeChannelDbModel YoutubeChannel { get; set; }

        public DateTime? PublishedAt { get; set; }

        public IEnumerable<YoutubeVideoThumbnailDbModel> Thumbnails { get; set; }

        public string ThumbnailsEtag { get; set; }

        public IReadOnlyCollection<YoutubeVideoTagDbModel> Tags { get; set; }

        public string YoutubeCategoryId { get; set; }

        public TimeSpan Duration { get; set; }

        public YoutubeVideoStatisticsDbModel Statistics { get; set; }

        public YoutubeVideoTopicDetailsDbModel TopicDetails { get; set; }
    }

    public class YoutubeChannelDbModel
    {
        public string Id { get; set; }

        public string Title { get; set; }
    }

    public class YoutubeVideoStatisticsDbModel
    {
        public string VideoId { get; set; }

        public ulong? ViewCount { get; set; }

        public ulong? LikeCount { get; set; }

        public ulong? DislikeCount { get; set; }

        public ulong? FavoriteCount { get; set; }

        public ulong? CommentCount { get; set; }
    }

    public class YoutubeVideoTopicDetailsDbModel
    {
        public string VideoId { get; set; }

        public IReadOnlyCollection<string> TopicIds { get; set; }

        public IReadOnlyCollection<string> RelevantTopicIds { get; set; }

        public IReadOnlyCollection<string> TopicCategories { get; set; }

        public string ETag { get; set; }
    }

    public class YoutubeVideoTagDbModel
    {
        public string VideoId { get; set; } 

        public string Value { get; set; }
    }

    public class YoutubeVideoThumbnailDbModel
    {
        public string VideoId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public long? Width { get; set; }

        public long? Height { get; set; }

        public string Etag { get; set; }
    }
}
