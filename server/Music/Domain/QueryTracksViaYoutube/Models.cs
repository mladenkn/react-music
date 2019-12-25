using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string YoutubeCategoryId { get; set; }

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

    public class YoutubeVideoProfile : Profile
    {
        public YoutubeVideoProfile()
        {
            CreateMap<Video, YoutubeVideoModel>()
                .ForMember(dst => dst.YoutubeCategoryId, o => o.MapFrom(src => src.Snippet.CategoryId))
                .ForMember(dst => dst.Duration, o => o.MapFrom(src => XmlConvert.ToTimeSpan(src.ContentDetails.Duration)))
                .IncludeMembers(src => src.Snippet)
                ;
            
            CreateMap<VideoSnippet, YoutubeVideoModel>(MemberList.None)
                .ForMember(dst => dst.Thumbnails, o => o.MapFrom((src, dst) =>
                {
                    var arr = new[]
                    {
                        (name: nameof(src.Thumbnails.Default__), thumbNail: src.Thumbnails.Default__),
                        (name: nameof(src.Thumbnails.High), thumbNail: src.Thumbnails.High),
                        (name: nameof(src.Thumbnails.Maxres), thumbNail: src.Thumbnails.Maxres),
                        (name: nameof(src.Thumbnails.Medium), thumbNail: src.Thumbnails.Medium),
                        (name: nameof(src.Thumbnails.Standard), thumbNail: src.Thumbnails.Standard),
                    };

                    return arr
                        .Where(item => item.thumbNail != null)
                        .Select(i => new YoutubeVideoThumbnailModel
                        {
                            Etag = i.thumbNail.ETag,
                            Height = i.thumbNail.Height,
                            Name = i.name,
                            Url = i.thumbNail.Url,
                            Width = i.thumbNail.Width,
                        });
                }))
                ;

            CreateMap<VideoStatistics, YoutubeVideoStatisticsModel>()
                ;

            CreateMap<VideoTopicDetails, YoutubeVideoTopicDetailsModel>()
                ;
        }
    }
}
