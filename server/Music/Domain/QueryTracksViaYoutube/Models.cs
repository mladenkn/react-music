using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AutoMapper;
using Google.Apis.YouTube.v3.Data;

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

        public string YoutubeCategoryId { get; set; }

        public TimeSpan Duration { get; set; }

        public YoutubeVideoStatisticsModel Statistics { get; set; }

        public YoutubeVideoTopicDetailsModel TopicDetails { get; set; }
    }

    public class YoutubeVideoStatisticsModel
    {
        public string VideoId { get; set; }

        public ulong? ViewCount { get; set; }

        public ulong? LikeCount { get; set; }

        public ulong? DislikeCount { get; set; }

        public ulong? FavoriteCount { get; set; }

        public ulong? CommentCount { get; set; }
    }

    public class YoutubeVideoTopicDetailsModel
    {
        public string VideoId { get; set; }

        public IReadOnlyCollection<string> TopicIds { get; set; }

        public IReadOnlyCollection<string> RelevantTopicIds { get; set; }

        public IReadOnlyCollection<string> TopicCategories { get; set; }

        public string ETag { get; set; }
    }

    public class YoutubeVideoThumbnailModel
    {
        public string VideoId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public long? Width { get; set; }

        public long? Height { get; set; }

        public string Etag { get; set; }
    }

    public class YoutubeVideoProfile : Profile
    {
        public YoutubeVideoProfile()
        {
            CreateMap<Video, YoutubeVideoModel>()
                .IncludeMembers(src => src.Snippet)
                .ForMember(dst => dst.YoutubeCategoryId, o => o.MapFrom(src => src.Snippet.CategoryId))
                .ForMember(dst => dst.ThumbnailsEtag, o => o.MapFrom(src => src.Snippet.Thumbnails.ETag))
                .ForMember(dst => dst.Duration, o => o.MapFrom(src => XmlConvert.ToTimeSpan(src.ContentDetails.Duration)))
                .IncludeMembers(src => src.Statistics)
                .IncludeMembers(src => src.TopicDetails)
                .AfterMap((src, dst) =>
                {
                    var thumbnailDetails = src.Snippet.Thumbnails;

                    var arr = new[]
                    {
                        (name: nameof(thumbnailDetails.Default__), thumbNail: thumbnailDetails.Default__),
                        (name: nameof(thumbnailDetails.High), thumbNail: thumbnailDetails.High),
                        (name: nameof(thumbnailDetails.Maxres), thumbNail: thumbnailDetails.Maxres),
                        (name: nameof(thumbnailDetails.Medium), thumbNail: thumbnailDetails.Medium),
                        (name: nameof(thumbnailDetails.Standard), thumbNail: thumbnailDetails.Standard),
                    };

                    dst.Thumbnails = arr
                        .Where(item => item.thumbNail != null)
                        .Select(i => new YoutubeVideoThumbnailModel
                        {
                            Etag = i.thumbNail.ETag,
                            Height = i.thumbNail.Height,
                            Name = i.name,
                            Url = i.thumbNail.Url,
                            Width = i.thumbNail.Width,
                        });
                })
                ;
        }
    }
}
