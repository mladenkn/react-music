using System;
using Google.Apis.YouTube.v3.Data;
using Music.DataAccess.Models;

namespace Executables
{
    public class DataGenerator
    {
        private int _nextInt = 1;

        public string String() => _nextInt++.ToString();

        public int Int() => _nextInt++;

        public YoutubeChannel YoutubeChannel(Action<YoutubeChannel> addMore = null)
        {
            var r = new YoutubeChannel
            {
                Title = String()
            };
            addMore?.Invoke(r);
            return r;
        }

        public User User(Action<User> addMore = null)
        {
            var r = new User
            {
                Email = String(),
            };
            addMore?.Invoke(r);
            return r;
        }

        public Track Track(Action<Track> addMore = null)
        {
            var track = new Track
            {
                Year = 1997,
            };
            addMore?.Invoke(track);
            return track;
        }

        public TrackTag TrackTag(Action<TrackTag> addMore = null)
        {
            var r = new TrackTag
            {
                Value = String(),
            };
            addMore?.Invoke(r);
            return r;
        }

        public YoutubeVideo YoutubeVideo(Action<YoutubeVideo> addMore = null)
        {
            var r = new YoutubeVideo
            {
                Description = String(),
                Duration = TimeSpan.FromSeconds(Int()),
                ThumbnailsEtag = String(),
                YoutubeCategoryId = String(),
                Title = String(),
                PublishedAt = DateTime.MinValue,
            };
            addMore?.Invoke(r);
            return r;
        }

        public Video Video(Action<Video> addMore = null)
        {
            var r = new Video
            {
            };
            addMore?.Invoke(r);
            return r;
        }

        public VideoSnippet VideoSnippet(Action<VideoSnippet> addMore = null)
        {
            var r = new VideoSnippet
            {
            };
            addMore?.Invoke(r);
            return r;
        }

        public YoutubeVideoTag YoutubeVideoTag(Action<YoutubeVideoTag> addMore = null)
        {
            var r = new YoutubeVideoTag
            {
                Value = String(),
            };
            addMore?.Invoke(r);
            return r;
        }

        public YoutubeVideoThumbnail YoutubeVideoThumbnail(Action<YoutubeVideoThumbnail> addMore = null)
        {
            var r = new YoutubeVideoThumbnail
            {
                Name = String(),
                Url = String(),
                Width = Int(),
                Height = Int(),
                Etag = String(),
            };
            addMore?.Invoke(r);
            return r;
        }

        public YoutubeVideoStatistics YoutubeVideoStatistics(Action<YoutubeVideoStatistics> addMore = null)
        {
            var r = new YoutubeVideoStatistics
            {
                ViewCount = (ulong) Int(),
                LikeCount = (ulong)Int(),
                DislikeCount = (ulong)Int(),
                FavoriteCount = (ulong)Int(),
                CommentCount = (ulong)Int(),
            };
            addMore?.Invoke(r);
            return r;
        }

        public YoutubeVideoTopicDetails YoutubeVideoTopicDetails(Action<YoutubeVideoTopicDetails> addMore = null)
        {
            var r = new YoutubeVideoTopicDetails
            {
                TopicIds = new []
                {
                    new YoutubeVideoTopicDetailsTopicId
                    {
                        Value = String(),
                    },
                    new YoutubeVideoTopicDetailsTopicId
                    {
                        Value = String(),
                    }
                },
                RelevantTopicIds = new[]
                {
                    new YoutubeVideoTopicDetailsRelevantTopicId
                    {
                        Value = String(),
                    },
                    new YoutubeVideoTopicDetailsRelevantTopicId
                    {
                        Value = String(),
                    }
                },
                TopicCategories = new[]
                {
                    new YoutubeVideoTopicDetailsTopicCategory
                    {
                        Value = String(),
                    },
                    new YoutubeVideoTopicDetailsTopicCategory
                    {
                        Value = String(),
                    }
                },
                ETag = String(),
            };
            addMore?.Invoke(r);
            return r;
        }
    }
}
