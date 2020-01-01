using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Music.DataAccess.Models;

namespace Music.Domain.Shared
{
    public class TrackModel
    {
        public string YoutubeVideoId { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public string YoutubeChannelId { get; set; }

        public string YoutubeChannelTitle { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }
    }

    public class TrackModelMapperProfile : Profile
    {
        public TrackModelMapperProfile()
        {
            var emptyTagsArray = new string[0];
            _ = CreateMap<YoutubeVideo, TrackModel>()
                .ForMember(dst => dst.YoutubeVideoId, o => o.MapFrom(src => src.Id))
                .ForMember(dst => dst.Image, o => o.MapFrom(src =>
                    src.Thumbnails.First(t => t.Name == "Default__").Url)
                )
                //.ForMember(dst => dst.Tags, o => o.MapFrom((src, dst, member, resContext) =>
                //    src.TrackUserProps.FirstOrDefault(t => t.UserId == (int)resContext.Items["userId"]) == null ?
                //        emptyTagsArray :
                //        src.TrackUserProps.FirstOrDefault(t => t.UserId == (int)resContext.Items["userId"])
                //            .TrackTags.Select(t => t.Value).ToArray()
                //))
                .ForMember(dst => dst.Tags, o => o.MapFrom(src =>
                    src.TrackUserProps.FirstOrDefault() == null ?
                        emptyTagsArray :
                        src.TrackUserProps.FirstOrDefault()
                            .TrackTags.Select(t => t.Value).ToArray()
                ))
                //.ForMember(dst => dst.Tags, o => o.MapFrom((src, dst, member, resContext) => emptyTagsArray))
                .ForMember(dst => dst.Year, o => o.MapFrom((src, dst, member, resContext) =>
                    src.TrackUserProps.FirstOrDefault(t => t.UserId == (int)resContext.Items["userId"])?.Year
                ))
                //.ForMember(dst => dst.Year, o => o.MapFrom(src =>
                //    src.TrackUserProps.FirstOrDefault() == null ? null : src.TrackUserProps.FirstOrDefault().Year
                //))
                //.ForMember(dst => dst.Year, o => o.MapFrom((src, dst, member, resContext) => 1990))
                ;
        }
    }
}
