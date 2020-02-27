using System.Linq;
using System.Xml;
using AutoMapper;
using Google.Apis.YouTube.v3.Data;
using Music.App.Models;

namespace Music.App.YouTubeVideos
{
    public class YoutubeVideoMapperProfile : Profile
    {
        public YoutubeVideoMapperProfile()
        {
            CreateMap<Video, YoutubeVideo>()
                .ForMember(dst => dst.YoutubeCategoryId, o => o.MapFrom(src => src.Snippet.CategoryId))
                .ForMember(dst => dst.YouTubeChannel, o => o.MapFrom(src => new YouTubeChannel
                {
                    Id = src.Snippet.ChannelId,
                    Title = src.Snippet.ChannelTitle,
                }))
                .ForMember(dst => dst.YoutubeChannelId, o => o.MapFrom(src => src.Snippet.ChannelId))
                .ForMember(dst => dst.Description, o => o.MapFrom(src => src.Snippet.Description))
                .ForMember(dst => dst.PublishedAt, o => o.MapFrom(src => src.Snippet.PublishedAt))
                .ForMember(dst => dst.Title, o => o.MapFrom(src => src.Snippet.Title))
                .ForMember(dst => dst.YoutubeCategoryId, o => o.MapFrom(src => src.Snippet.CategoryId))
                .ForMember(dst => dst.Duration, o => o.MapFrom(src => XmlConvert.ToTimeSpan(src.ContentDetails.Duration)))
                .ForMember(dst => dst.Thumbnails, o => o.MapFrom((src, dst) =>
                {
                    var src_ = src.Snippet.Thumbnails;

                    var arr = new[]
                    {
                        (name: nameof(src_.Default__), thumbNail: src_.Default__),
                        (name: nameof(src_.High), thumbNail: src_.High),
                        (name: nameof(src_.Maxres), thumbNail: src_.Maxres),
                        (name: nameof(src_.Medium), thumbNail: src_.Medium),
                        (name: nameof(src_.Standard), thumbNail: src_.Standard),
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
                }))
                .ForMember(dst => dst.ThumbnailsEtag, o => o.MapFrom(src => src.Snippet.Thumbnails.ETag))
                .ForMember(dst => dst.TrackUserProps, o => o.Ignore())
                .ForMember(dst => dst.Tags, o => o.MapFrom(src => src.Snippet.Tags.Select(t => new YoutubeVideoTag
                {
                    Value = t
                })))
                .AfterMap((src, dst) =>
                {
                    dst.Statistics.YoutubeVideoId = src.Id;

                    if (dst.TopicDetails != null)
                    {
                        dst.TopicDetails.YoutubeVideoId = src.Id;
                        foreach (var tc in dst.TopicDetails.TopicCategories)
                            tc.YoutubeVideoId = src.Id;
                        foreach (var rtId in dst.TopicDetails.RelevantTopicIds)
                            rtId.YoutubeVideoId = src.Id;
                        foreach (var tId in dst.TopicDetails.TopicIds)
                            tId.YoutubeVideoId = src.Id;
                    }
                })
                ;
            ;

            CreateMap<VideoStatistics, YoutubeVideoStatistics>()
                .ForMember(dst => dst.Id, o => o.Ignore())
                .ForMember(dst => dst.YoutubeVideoId, o => o.Ignore())
                ;

            CreateMap<VideoTopicDetails, YoutubeVideoTopicDetails>()
                .ForMember(dst => dst.Id, o => o.Ignore())
                .ForMember(dst => dst.YoutubeVideoId, o => o.Ignore())
                .ForMember(dst => dst.RelevantTopicIds, 
                    o => o.MapFrom(src => src.RelevantTopicIds.Select(rt => new YoutubeVideoTopicDetailsRelevantTopicId
                    {
                        Value = rt
                    })))
                .ForMember(dst => dst.TopicCategories,
                    o => o.MapFrom(src => src.RelevantTopicIds.Select(rt => new YoutubeVideoTopicDetailsTopicCategory
                    {
                        Value = rt
                    })))
                .ForMember(dst => dst.TopicIds,
                    o => o.MapFrom(src => src.RelevantTopicIds.Select(rt => new YoutubeVideoTopicDetailsTopicId
                    {
                        Value = rt
                    })))
                ;
        }
    }
}
