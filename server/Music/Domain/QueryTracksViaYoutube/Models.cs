using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AutoMapper;
using Google.Apis.YouTube.v3.Data;
using Music.DataAccess.Models;

namespace Music.Domain.QueryTracksViaYoutube
{
    public class YoutubeVideoModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ChannelId { get; set; }

        public string ChannelTitle { get; set; }

        public DateTime? PublishedAt { get; set; }

        public IEnumerable<YoutubeVideoThumbnailModel> Thumbnails { get; set; }

        public string ThumbnailsEtag { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string CategoryId { get; set; }

        public TimeSpan Duration { get; set; }

        public YoutubeVideoStatisticsModel Statistics { get; set; }

        public YoutubeVideoTopicDetailsModel TopicDetails { get; set; }
    }

    public class YoutubeVideoStatisticsModel
    {
        public ulong? ViewCount { get; set; }

        public ulong? LikeCount { get; set; }

        public ulong? DislikeCount { get; set; }

        public ulong? FavoriteCount { get; set; }

        public ulong? CommentCount { get; set; }
    }

    public class YoutubeVideoTopicDetailsModel
    {
        public IReadOnlyCollection<string> TopicIds { get; set; }

        public IReadOnlyCollection<string> RelevantTopicIds { get; set; }

        public IReadOnlyCollection<string> TopicCategories { get; set; }

        public string ETag { get; set; }
    }

    public class YoutubeVideoThumbnailModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public long? Width { get; set; }

        public long? Height { get; set; }

        public string Etag { get; set; }
    }
}
