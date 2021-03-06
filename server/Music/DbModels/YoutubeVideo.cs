﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kernel;

namespace Music.DbModels
{
    public class YoutubeVideo : IDatabaseEntity
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string YoutubeChannelId { get; set; }

        public long? TrackId { get; set; }

        [Required]
        public YouTubeVideoCategory Category { get; set; }

        public Track Track { get; set; }

        public YouTubeChannel YouTubeChannel { get; set; }

        public DateTime? PublishedAt { get; set; }

        public IEnumerable<YoutubeVideoThumbnail> Thumbnails { get; set; }

        public string ThumbnailsEtag { get; set; }

        public IReadOnlyCollection<YoutubeVideoTag> Tags { get; set; }

        [Required]
        public string YoutubeCategoryId { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        public DateTime LastUpdateAt { get; set; }

        public YoutubeVideoStatistics Statistics { get; set; }

        public YoutubeVideoTopicDetails TopicDetails { get; set; }
    }

    public enum YouTubeVideoCategory
    {
        Track, Compilation, Other
    }

    public class YoutubeVideoStatistics
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string YoutubeVideoId { get; set; }

        public ulong? ViewCount { get; set; }

        public ulong? LikeCount { get; set; }

        public ulong? DislikeCount { get; set; }

        public ulong? FavoriteCount { get; set; }

        public ulong? CommentCount { get; set; }

        public string ETag { get; set; }
    }

    public class YoutubeVideoTopicDetails
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string YoutubeVideoId { get; set; }

        public IReadOnlyCollection<YoutubeVideoTopicDetailsTopicId> TopicIds { get; set; }

        public IReadOnlyCollection<YoutubeVideoTopicDetailsRelevantTopicId> RelevantTopicIds { get; set; }

        public IReadOnlyCollection<YoutubeVideoTopicDetailsTopicCategory> TopicCategories { get; set; }

        public string ETag { get; set; }
    }

    public class YoutubeVideoTopicDetailsTopicId
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string YoutubeVideoId { get; set; }

        [Required]
        public string Value { get; set; }
    }

    public class YoutubeVideoTopicDetailsRelevantTopicId
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string YoutubeVideoId { get; set; }

        [Required]
        public string Value { get; set; }
    }

    public class YoutubeVideoTopicDetailsTopicCategory
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string YoutubeVideoId { get; set; }

        [Required]
        public string Value { get; set; }
    }

    public class YoutubeVideoTag
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string YoutubeVideoId { get; set; }

        [Required]
        public string Value { get; set; }
    }

    public class YoutubeVideoThumbnail
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string YoutubeVideoId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        public long? Width { get; set; }

        public long? Height { get; set; }

        public string Etag { get; set; }
    }
}
