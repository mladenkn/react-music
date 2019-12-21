using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.YouTube.v3.Data;

namespace Music.Models
{
    public class YoutubeVideo
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public string ChannelId { get; set; }

        public string ChannelTitle { get; set; }

        public DateTime? PublishedAt { get; set; }

        public IEnumerable<YoutubeVideoThumbnail> Thumbnails { get; set; }

        public string ThumbnailsEtag { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string YoutubeCategoryId { get; set; }

        public TimeSpan Duration { get; set; }

        public YoutubeVideoStatistics Statistics { get; set; }

        public YoutubeVideoTopicDetails TopicDetails { get; set; }
    }

    public class YoutubeVideoStatistics
    {
        public ulong? ViewCount { get; set; }

        public ulong? LikeCount { get; set; }
        
        public ulong? DislikeCount { get; set; }

        public ulong? FavoriteCount { get; set; }
        
        public ulong? CommentCount { get; set; }
    }

    public class YoutubeVideoTopicDetails
    {
        public IReadOnlyCollection<string> TopicIds { get; set; }

        public IReadOnlyCollection<string> RelevantTopicIds { get; set; }

        public IReadOnlyCollection<string> TopicCategories { get; set; }

        public string ETag { get; set; }
    }

    public class YoutubeVideoThumbnail
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public long? Width { get; set; }

        public long? Height { get; set; }

        public string Etag { get; set; }

        public static IEnumerable<YoutubeVideoThumbnail> CreateCollection(ThumbnailDetails thumbnailDetails)
        {
            var arr = new[]
            {
                (name: nameof(thumbnailDetails.Default__), thumbNail: thumbnailDetails.Default__),
                (name: nameof(thumbnailDetails.High), thumbNail: thumbnailDetails.High),
                (name: nameof(thumbnailDetails.Maxres), thumbNail: thumbnailDetails.Maxres),
                (name: nameof(thumbnailDetails.Medium), thumbNail: thumbnailDetails.Medium),
                (name: nameof(thumbnailDetails.Standard), thumbNail: thumbnailDetails.Standard),
            };

            return arr
                .Where(item => item.thumbNail != null)
                .Select(i => new YoutubeVideoThumbnail
                {
                    Etag = i.thumbNail.ETag,
                    Height = i.thumbNail.Height,
                    Name = i.name,
                    Url = i.thumbNail.Url,
                    Width = i.thumbNail.Width,
                });
        }
    }
}
