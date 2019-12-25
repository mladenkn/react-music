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

    public class YoutubeVideoProfile : Profile
    {
        public YoutubeVideoProfile()
        {
            CreateMap<Video, YoutubeVideoModel>()
                .IncludeMembers(src => src.Snippet)
                .ForMember(dst => dst.Duration, o => o.MapFrom(src => XmlConvert.ToTimeSpan(src.ContentDetails.Duration)))
                ;
            
            CreateMap<VideoSnippet, YoutubeVideoModel>()
                .ForMember(dst => dst.Id, o => o.Ignore())
                .ForMember(dst => dst.Duration, o => o.Ignore())
                .ForMember(dst => dst.Statistics, o => o.Ignore())
                .ForMember(dst => dst.TopicDetails, o => o.Ignore())
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
            

            CreateMap<YoutubeVideoModel, YoutubeVideo>()
                .ForMember(dst => dst.YoutubeChannelId, o => o.MapFrom(src => src.ChannelId))
                .ForPath(dst => dst.YoutubeChannel.Id, o => o.MapFrom(src => src.ChannelId))
                .ForPath(dst => dst.YoutubeChannel.Title, o => o.MapFrom(src => src.ChannelTitle))
                .ForMember(dst => dst.YoutubeCategoryId, o => o.MapFrom(src => src.CategoryId))
                .ForMember(dst => dst.Tags, o => o.MapFrom(src => src.Tags.Select(t => new YoutubeVideoTag
                {
                    Value = t,
                    YoutubeVideoId = src.Id
                })))
                .ForMember(dst => dst.TrackUserPropsId, o => o.Ignore())
                .ForMember(dst => dst.TrackUserProps, o => o.Ignore())
                ;

            CreateMap<YoutubeVideo, YoutubeVideoModel>()
                .ForMember(dst => dst.ChannelId, o => o.MapFrom(src => src.YoutubeChannelId))
                .ForMember(dst => dst.ChannelTitle, o => o.MapFrom(src => src.YoutubeChannel.Title))
                .ForMember(dst => dst.CategoryId, o => o.MapFrom(src => src.YoutubeCategoryId))
                ;

            CreateMap<YoutubeVideoStatisticsModel, YoutubeVideoStatistics>()
                .ForMember(dst => dst.YoutubeVideoId, o => o.Ignore())
                .ForMember(dst => dst.Id, o => o.Ignore())
                .ReverseMap()
                ;

            CreateMap<YoutubeVideoTopicDetailsModel, YoutubeVideoTopicDetails>()
                .ForMember(dst => dst.YoutubeVideoId, o => o.Ignore())
                .ForMember(dst => dst.Id, o => o.Ignore())
                .ForMember(dst => dst.RelevantTopicIds, o => o.MapFrom(src => src.RelevantTopicIds.Select(tId => new YoutubeVideoTopicDetailsRelevantTopicId
                {
                    Value = tId,
                })))
                .ForMember(dst => dst.TopicCategories, o => o.MapFrom(src => src.TopicCategories.Select(tId => new YoutubeVideoTopicDetailsTopicCategories
                {
                    Value = tId,
                })))
                .ForMember(dst => dst.TopicIds, o => o.MapFrom(src => src.TopicIds.Select(tId => new YoutubeVideoTopicDetailsTopicIds
                {
                    Value = tId,
                })))
                ;

            CreateMap<YoutubeVideoTopicDetails, YoutubeVideoTopicDetailsModel>()
                .ForMember(dst => dst.RelevantTopicIds, o => o.MapFrom(src => src.RelevantTopicIds.Select(t => t.Value)))
                .ForMember(dst => dst.TopicCategories, o => o.MapFrom(src => src.TopicCategories.Select(t => t.Value)))
                .ForMember(dst => dst.TopicIds, o => o.MapFrom(src => src.TopicIds.Select(t => t.Value)))
                ;

            CreateMap<YoutubeVideoThumbnailModel, YoutubeVideoThumbnail>()
                .ForMember(dst => dst.YoutubeVideoId, o => o.Ignore())
                .ForMember(dst => dst.Id, o => o.Ignore())
                .ReverseMap()
                ;
        }
    }
}
