using System;
using System.Collections.Generic;

namespace Music.DataAccess.Models
{
    public class YoutubeVideo
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string YoutubeChannelId { get; set; }

        public YoutubeChannel YoutubeChannel { get; set; }

        public DateTime? PublishedAt { get; set; }

        public IEnumerable<YoutubeVideoThumbnail> Thumbnails { get; set; }

        public string ThumbnailsEtag { get; set; }

        public IReadOnlyCollection<YoutubeVideoTag> Tags { get; set; }

        public string YoutubeCategoryId { get; set; }

        public TimeSpan Duration { get; set; }

        public YoutubeVideoStatistics Statistics { get; set; }

        public YoutubeVideoTopicDetails TopicDetails { get; set; }
    }

    public class YoutubeChannel
    {
        public string Id { get; set; }

        public string Title { get; set; }
    }

    public class YoutubeVideoStatistics
    {
        public int Id { get; set; }

        public string YoutubeVideoId { get; set; }

        public ulong? ViewCount { get; set; }

        public ulong? LikeCount { get; set; }

        public ulong? DislikeCount { get; set; }

        public ulong? FavoriteCount { get; set; }

        public ulong? CommentCount { get; set; }
    }

    public class YoutubeVideoTopicDetails
    {
        public int Id { get; set; }

        public string YoutubeVideoId { get; set; }

        public IReadOnlyCollection<YoutubeVideoTopicDetailsTopicIds> TopicIds { get; set; }

        public IReadOnlyCollection<YoutubeVideoTopicDetailsRelevantTopicId> RelevantTopicIds { get; set; }

        public IReadOnlyCollection<YoutubeVideoTopicDetailsTopicCategories> TopicCategories { get; set; }

        public string ETag { get; set; }
    }

    public class YoutubeVideoTopicDetailsTopicIds
    {
        public int Id { get; set; }

        public string YoutubeVideoId { get; set; }

        public string Value { get; set; }
    }

    public class YoutubeVideoTopicDetailsRelevantTopicId
    {
        public int Id { get; set; }

        public string YoutubeVideoId { get; set; }

        public string Value { get; set; }
    }

    public class YoutubeVideoTopicDetailsTopicCategories
    {
        public int Id { get; set; }

        public string YoutubeVideoId { get; set; }

        public string Value { get; set; }
    }

    public class YoutubeVideoTag
    {
        public int Id { get; set; }

        public string YoutubeVideoId { get; set; } 

        public string Value { get; set; }
    }

    public class YoutubeVideoThumbnail
    {
        public int Id { get; set; }

        public string YoutubeVideoId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public long? Width { get; set; }

        public long? Height { get; set; }

        public string Etag { get; set; }
    }
}
