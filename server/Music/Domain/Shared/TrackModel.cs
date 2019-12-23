using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Music.DataAccess.Models;

namespace Music.Domain.Shared
{
    public class TrackModel
    {
        public string YtId { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public string YoutubeChannelId { get; set; }

        public string YoutubeChannelTitle { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }
    }

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<DataAccess.Models.TrackUserProps, TrackModel>()
                .ForMember(dst => dst.YtId, o => o.MapFrom(src => src.YoutubeVideoId))
                .IncludeMembers(src => src.YoutubeVideo)
                .ForMember(dst => dst.Image, o => o.MapFrom(src =>
                    src.YoutubeVideo.Thumbnails.First(t => t.Name == "Default__").Url)
                )
                .ForMember(dst => dst.Tags, o => o.MapFrom(src => src.TrackUserPropsTags.Select(t => t.Value)))
                ;
        }
    }
}
