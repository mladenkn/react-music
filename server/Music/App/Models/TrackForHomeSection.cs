using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Music.App.DbModels;

namespace Music.App.Models
{
    public class TrackForHomeSection
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

        public static Expression<Func<TrackUserProps, TrackForHomeSection>> FromTrackUserProps => trackUserProps =>
            new TrackForHomeSection
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

        public static Expression<Func<Track, TrackForHomeSection>> FromTrack(int userId) => track =>
            new TrackForHomeSection
            {
                Id = track.Id,
                YouTubeVideoId = track.YoutubeVideos.FirstOrDefault().Id,
                Title = track.YoutubeVideos.FirstOrDefault().Title,
                Description = track.YoutubeVideos.FirstOrDefault().Description,
                Image = track.YoutubeVideos.FirstOrDefault().Thumbnails.First(t => t.Name == "Default__").Url,
                YoutubeChannelId = track.YoutubeVideos.FirstOrDefault().YoutubeChannelId,
                YoutubeChannelTitle = track.YoutubeVideos.FirstOrDefault().YouTubeChannel.Title,
                Year = track.TrackUserProps.FirstOrDefault(t => t.UserId == userId) == null ?
                    null :
                    track.TrackUserProps.FirstOrDefault(t => t.UserId == userId).Year,
                Tags = track.TrackUserProps.FirstOrDefault(t => t.UserId == userId) == null ?
                    EmptyTagsArray :
                    track.TrackUserProps.FirstOrDefault(t => t.UserId == userId).TrackTags
                        .Select(t => t.Value)
                        .ToArray(),
            };

        private static string[] EmptyTagsArray = new string[0];
    }
}
