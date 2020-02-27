using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Music.App.Models
{
    [DatabaseEntity]
    public class YoutubeVideo
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string YoutubeChannelId { get; set; }

        public YoutubeChannel YoutubeChannel { get; set; }

        public DateTime? PublishedAt { get; set; }

        public IEnumerable<YoutubeVideoThumbnail> Thumbnails { get; set; }

        public string ThumbnailsEtag { get; set; }

        public IReadOnlyCollection<YoutubeVideoTag> Tags { get; set; }

        [Required]
        public string YoutubeCategoryId { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        public YoutubeVideoStatistics Statistics { get; set; }

        public YoutubeVideoTopicDetails TopicDetails { get; set; }

        public IReadOnlyCollection<TrackUserProps> TrackUserProps { get; set; }

        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YoutubeVideo>()
                .HasMany(v => v.TrackUserProps)
                .WithOne(t => t.YoutubeVideo)
                ;
        }
    }

    public class YoutubeChannel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }
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
