using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Music.DataAccess.Models;
using Utilities;

namespace Executables
{
    public interface IDataGenerator
    {
        YoutubeChannel YoutubeChannel(params Expression<Func<YoutubeChannel, object>>[] parts);
        User User(params Expression<Func<User, object>>[] parts);
        Track Track(params Expression<Func<Track, object>>[] parts);
        TrackTag TrackTag(params Expression<Func<TrackTag, object>>[] parts);
        YoutubeVideo YoutubeVideo(params Expression<Func<YoutubeVideo, object>>[] parts);
    }

    public class DataGenerator : IDataGenerator
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

        public YoutubeChannel YoutubeChannel(params Expression<Func<YoutubeChannel, object>>[] parts)
        {
            return new YoutubeChannel
            {
                Id = PartsContain(parts, c => c.Id) ? NextString() : null,
                Title = NextString()
            };
        }

        public User User(params Expression<Func<User, object>>[] parts)
        {
            return new User
            {
                Id = PartsContain(parts, c => c.Id) ? NextInt() : 0,
                Email = NextString(),
            };
        }

        public Track Track(params Expression<Func<Track, object>>[] parts)
        {
            throw new NotImplementedException();
        }

        public YoutubeVideo YoutubeVideo(params Expression<Func<YoutubeVideo, object>>[] parts)
        {
            var video = new YoutubeVideo
            {
                Id = PartsContain(parts, v => v.Id) ? NextString() : null,
                Description = NextString(),
                Duration = TimeSpan.FromSeconds(2),
                ThumbnailsEtag = NextString(),
                YoutubeCategoryId = NextString(),
                Title = NextString(),
            };

            if (PartsContain(parts, v => v.YoutubeChannelId))
            {
                video.YoutubeChannelId = NextString();
                if (PartsContain(parts, v => v.YoutubeChannel))
                {
                    video.YoutubeChannel = new YoutubeChannel {Title = NextString()};
                    if (PartsContain(parts, v => v.YoutubeChannel.Id))
                        video.YoutubeChannel.Id = NextString();
                }
            }

            return video;
        }

        public TrackTag TrackTag(params Expression<Func<TrackTag, object>>[] parts)
        {
            return new TrackTag
            {
                TrackId = PartsContain(parts, c => c.TrackId) ? NextInt() : 0,
                Value = NextString(),
            };
        }
    }
}
