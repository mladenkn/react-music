using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Music.App.Models
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

        public static Expression<Func<YoutubeVideo, TrackModel>> FromYoutubeVideo(int userId) => ytVideo =>
            new TrackModel
            {
                YoutubeVideoId = ytVideo.Id,
                Title = ytVideo.Title,
                Description = ytVideo.Description,
                Image = ytVideo.Thumbnails.First(t => t.Name == "Default__").Url,
                YoutubeChannelId = ytVideo.YoutubeChannelId,
                YoutubeChannelTitle = ytVideo.YoutubeChannel.Title,
                Year = ytVideo.TrackUserProps.FirstOrDefault(t => t.UserId == userId) == null ?
                    null :
                    ytVideo.TrackUserProps.FirstOrDefault(t => t.UserId == userId).Year,
                Tags = ytVideo.TrackUserProps.FirstOrDefault(t => t.UserId == userId) == null ?
                    EmptyTagsArray :
                    ytVideo.TrackUserProps.FirstOrDefault(t => t.UserId == userId).TrackTags
                        .Select(t => t.Value)
                        .ToArray(),
            };

        public static Expression<Func<TrackUserProps, TrackModel>> FromTrackUserProps => trackUserProps =>
            new TrackModel
            {
                YoutubeVideoId = trackUserProps.YoutubeVideoId,
                Title = trackUserProps.YoutubeVideo.Title,
                Description = trackUserProps.YoutubeVideo.Description,
                Image = trackUserProps.YoutubeVideo.Thumbnails.First(t => t.Name == "Default__").Url,
                YoutubeChannelId = trackUserProps.YoutubeVideo.YoutubeChannelId,
                YoutubeChannelTitle = trackUserProps.YoutubeVideo.YoutubeChannel.Title,
                Year = trackUserProps.Year,
                Tags = trackUserProps.TrackTags.Select(t => t.Value).ToArray(),
            };

        private static string[] EmptyTagsArray = new string[0];
    }
}
