using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.EntityFrameworkCore.InMemory.Query.Internal;
using Music.Models;
using Utilities;

namespace Music.Repositories
{
    public class YoutubeDataApiVideoRepository
    {
        private readonly YouTubeService _youTubeService;

        public YoutubeDataApiVideoRepository(YouTubeService youTubeService)
        {
            _youTubeService = youTubeService;
        }

        public async Task<IReadOnlyCollection<YoutubeVideo>> GetList(IReadOnlyCollection<string> ids)
        {
            var r = new List<YoutubeVideo>(ids.Count);

            var chunkCount = ids.Count < 50 ? 1 : ids.Count / 50;

            foreach (var idsChunk in ids.Batch(chunkCount))
            {
                var allTracksFromYtRequest = _youTubeService.Videos.List("snippet,contentDetails,statistics,topicDetails");
                allTracksFromYtRequest.Id = string.Join(",", idsChunk);
                var allTracksFromYt = await allTracksFromYtRequest.ExecuteAsync();
                r.AddRange(allTracksFromYt.Items.Select(MapToYoutubeVideo));
            }

            return r;
        }
        
        private static YoutubeVideo MapToYoutubeVideo(Video fromYt)
        {
            var topicDetails = fromYt.TopicDetails;

            return new YoutubeVideo
            {
                Id = fromYt.Id,
                Title = fromYt.Snippet.Title,
                Description = fromYt.Snippet.Description,
                ChannelId = fromYt.Snippet.ChannelId,
                ChannelTitle = fromYt.Snippet.ChannelTitle,
                PublishedAt = fromYt.Snippet.PublishedAt,
                Tags = fromYt.Snippet.Tags,
                YoutubeCategoryId = fromYt.Snippet.CategoryId,
                Thumbnails = YoutubeVideoThumbnail.CreateCollection(fromYt.Snippet.Thumbnails),
                ThumbnailsEtag = fromYt.Snippet.Thumbnails.ETag,
                Duration = XmlConvert.ToTimeSpan(fromYt.ContentDetails.Duration),
                Statistics = new YoutubeVideoStatistics
                {
                    CommentCount = fromYt.Statistics.CommentCount,
                    LikeCount = fromYt.Statistics.LikeCount,
                    DislikeCount = fromYt.Statistics.DislikeCount,
                    FavoriteCount = fromYt.Statistics.FavoriteCount,
                    ViewCount = fromYt.Statistics.ViewCount,
                },
                TopicDetails = topicDetails == null ? null : new YoutubeVideoTopicDetails
                {
                    RelevantTopicIds = topicDetails.RelevantTopicIds == null ? new string[0] : topicDetails.RelevantTopicIds.ToArray(),
                    TopicCategories = topicDetails.TopicCategories == null ? new string[0] : topicDetails.TopicCategories.ToArray(),
                    TopicIds = topicDetails.TopicIds == null ? new string[0] : topicDetails.TopicIds.ToArray(),
                    ETag = fromYt.TopicDetails.ETag,
                }
            };
        }

        //private static TimeSpan ParseDuration(string durationString)
        //{
        //    var indexOfH = durationString.IndexOf('H');
        //    var indexOfM = durationString.IndexOf('M');
        //    var indexOfS = durationString.IndexOf('S');

        //    try
        //    {
        //        var minutesLowerBound = indexOfH == -1 ? durationString.IndexOf('T') : indexOfH;

        //        int secs;
        //        if (indexOfS != -1)
        //        {
        //            var secsString = durationString.SubstringBetweenIndexes(indexOfM + 1, indexOfS);
        //            secs = int.Parse(secsString);
        //        }
        //        else
        //            secs = 0;

        //        var minutesString = durationString.SubstringBetweenIndexes(minutesLowerBound + 1, indexOfM);

        //        var minutes = int.Parse(minutesString);
        //        var secsTotal = secs + minutes * 60;
        //        var r = TimeSpan.FromSeconds(secsTotal);
        //        return r;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
    }
}
