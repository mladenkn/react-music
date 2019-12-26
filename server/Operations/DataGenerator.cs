using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Music.DataAccess.Models;
using Utilities;

namespace Executables
{
    public class DataGenerator
    {
        private int _nextInt = 1;

        private string NextString() => _nextInt++.ToString();

        private int NextInt() => _nextInt++;

        private static bool PartsContain<T>(IEnumerable<Expression<Func<T, object>>> allParts,
            Expression<Func<T, object>> part)
        {
            var allPartStrings = allParts.Select(ReflectionUtils.GetPropertyPath);
            var partString = ReflectionUtils.GetPropertyPath(part);
            return allPartStrings.Contains(partString);
        }

        private static bool PartsContain<T>(IEnumerable<string> allPartStrings, Expression<Func<T, object>> part)
        {
            var partString = ReflectionUtils.GetPropertyPath(part);
            return allPartStrings.Contains(partString);
        }

        public YoutubeChannel YoutubeChannel(Action<ListBuilder<Expression<Func<YoutubeChannel, object>>>> addParts, Action<YoutubeChannel> addMore = null)
        {
            var parts = ListBuilder.Build(addParts);
            var r = new YoutubeChannel
            {
                Id = PartsContain(parts, c => c.Id) ? NextString() : null,
                Title = NextString()
            };
            addMore?.Invoke(r);
            return r;
        }

        public User User(Action<ListBuilder<Expression<Func<User, object>>>> addParts, Action<User> addMore = null)
        {
            var parts = ListBuilder.Build(addParts);
            var r = new User
            {
                Id = PartsContain(parts, c => c.Id) ? NextInt() : 0,
                Email = NextString(),
            };
            addMore?.Invoke(r);
            return r;
        }

        public Track Track(params Expression<Func<Track, object>>[] parts)
        {
            throw new NotImplementedException();
        }

        public TrackTag TrackTag(Action<ListBuilder<Expression<Func<TrackTag, object>>>> addParts, Action<TrackTag> addMore = null)
        {
            var parts = ListBuilder.Build(addParts);
            var r = new TrackTag
            {
                TrackId = PartsContain(parts, c => c.TrackId) ? NextInt() : 0,
                Value = NextString(),
            };
            addMore?.Invoke(r);
            return r;
        }

        public YoutubeVideo YoutubeVideo(Action<ListBuilder<Expression<Func<YoutubeVideo, object>>>> addParts, Action<YoutubeVideo> addMore = null)
        {
            var parts = ListBuilder.Build(addParts);
            var r = new YoutubeVideo
            {
                Id = PartsContain(parts, v => v.Id) ? NextString() : null,
                Description = NextString(),
                Duration = TimeSpan.FromSeconds(NextInt()),
                ThumbnailsEtag = NextString(),
                YoutubeCategoryId = NextString(),
                Title = NextString(),
                YoutubeChannelId = PartsContain(parts, v => v.YoutubeChannelId) ? NextString() : null,
                PublishedAt = DateTime.MinValue,
            };
            addMore?.Invoke(r);
            return r;
        }

        public YoutubeVideoTag YoutubeVideoTag(Action<ListBuilder<Expression<Func<YoutubeVideoTag, object>>>> addParts = null, Action<YoutubeVideoTag> addMore = null)
        {
            var parts = addParts != null ? ListBuilder.Build(addParts) : new List<Expression<Func<YoutubeVideoTag, object>>>();
            var r = new YoutubeVideoTag
            {
                Value = NextString(),
            };
            addMore?.Invoke(r);
            return r;
        }

        public YoutubeVideoThumbnail YoutubeVideoThumbnail(Action<ListBuilder<Expression<Func<YoutubeVideoThumbnail, object>>>> addParts = null, Action<YoutubeVideoThumbnail> addMore = null)
        {
            var parts = addParts != null ? ListBuilder.Build(addParts) : new List<Expression<Func<YoutubeVideoThumbnail, object>>>();
            var r = new YoutubeVideoThumbnail
            {
                Name = NextString(),
                Url = NextString(),
                Width = NextInt(),
                Height = NextInt(),
                Etag = NextString(),
            };
            addMore?.Invoke(r);
            return r;
        }

        public YoutubeVideoStatistics YoutubeVideoStatistics(Action<ListBuilder<Expression<Func<YoutubeVideoStatistics, object>>>> addParts = null, Action<YoutubeVideoStatistics> addMore = null)
        {
            var parts = addParts != null ? ListBuilder.Build(addParts) : new List<Expression<Func<YoutubeVideoStatistics, object>>>();
            var r = new YoutubeVideoStatistics
            {
                ViewCount = (ulong) NextInt(),
                LikeCount = (ulong)NextInt(),
                DislikeCount = (ulong)NextInt(),
                FavoriteCount = (ulong)NextInt(),
                CommentCount = (ulong)NextInt(),
            };
            addMore?.Invoke(r);
            return r;
        }

        public YoutubeVideoTopicDetails YoutubeVideoTopicDetails(Action<ListBuilder<Expression<Func<YoutubeVideoTopicDetails, object>>>> addParts = null, Action<YoutubeVideoTopicDetails> addMore = null)
        {
            var parts = addParts != null ? ListBuilder.Build(addParts) : new List<Expression<Func<YoutubeVideoTopicDetails, object>>>();
            var r = new YoutubeVideoTopicDetails
            {
                TopicIds = new []
                {
                    new YoutubeVideoTopicDetailsTopicId
                    {
                        Value = NextString(),
                    },
                    new YoutubeVideoTopicDetailsTopicId
                    {
                        Value = NextString(),
                    }
                },
                RelevantTopicIds = new[]
                {
                    new YoutubeVideoTopicDetailsRelevantTopicId
                    {
                        Value = NextString(),
                    },
                    new YoutubeVideoTopicDetailsRelevantTopicId
                    {
                        Value = NextString(),
                    }
                },
                TopicCategories = new[]
                {
                    new YoutubeVideoTopicDetailsTopicCategory
                    {
                        Value = NextString(),
                    },
                    new YoutubeVideoTopicDetailsTopicCategory
                    {
                        Value = NextString(),
                    }
                },
                ETag = NextString(),
            };
            addMore?.Invoke(r);
            return r;
        }
    }

    public class ListBuilder<T>
    {
        private readonly List<T> _list = new List<T>();

        public ListBuilder<T> Add(T value)
        {
            _list.Add(value);
            return this;
        }

        public List<T> Build() => _list;
    }

    public static class ListBuilder
    {
        public static List<T> Build<T>(Action<ListBuilder<T>> addItems)
        {
            var instance = new ListBuilder<T>();
            addItems(instance);
            return instance.Build();
        }
    }
}
