using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Music.App.DbModels;

namespace Music.App.Models
{
    public class TrackModel
    {
        public long Id { get; set; }

        public string YouTubeVideoId { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public string YoutubeChannelId { get; set; }

        public string YoutubeChannelTitle { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }

        public static Expression<Func<TrackUserProps, TrackModel>> FromTrackUserProps => trackUserProps =>
            new TrackModel
            {
                Id = trackUserProps.TrackId,
                YouTubeVideoId = trackUserProps.YoutubeVideoId,
                Title = trackUserProps.YoutubeVideo.Title,
                Description = trackUserProps.YoutubeVideo.Description,
                Image = trackUserProps.YoutubeVideo.Thumbnails.First(t => t.Name == "Default__").Url,
                YoutubeChannelId = trackUserProps.YoutubeVideo.YoutubeChannelId,
                YoutubeChannelTitle = trackUserProps.YoutubeVideo.YouTubeChannel.Title,
                Year = trackUserProps.Year,
                Tags = trackUserProps.TrackTags.Select(t => t.Value).ToArray(),
            };

        private static string[] EmptyTagsArray = new string[0];
    }
}
