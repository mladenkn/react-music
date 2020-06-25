using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;
using Microsoft.EntityFrameworkCore;
using Music.DbModels;
using Utilities;

namespace Music.Services
{
    public class YouTubeVideosService : ServiceResolverAware
    {
        public YouTubeVideosService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IReadOnlyList<YoutubeVideo>> Get(IEnumerable<string> ids) =>
            await Query<YoutubeVideo>()
                .Where(v => ids.Contains(v.Id))
                .ToArrayAsync();

        public async Task<IReadOnlyList<YoutubeVideo>> EnsureAreSavedIfFound(IEnumerable<string> possibleIds)
        {
            var newVideosIds = (await FilterToUnknownVideosIds(possibleIds)).ToArray();
            var newVideos = await Resolve<YouTubeRemoteService>().GetByIdsIfFound(newVideosIds, new [] { "snippet", "contentDetails", "statistics", "topicDetails" });
            var newVideosMapped = await MapToYouTubeVideos(newVideos);
            await Save(newVideosMapped);
            return newVideosMapped;
        }

        private async Task<YoutubeVideo[]> MapToYouTubeVideos(IReadOnlyCollection<Video> vids)
        {
            foreach (var videoFromYt in vids)
            {
                if (videoFromYt.ContentDetails == null)
                    throw new Exception("Video from YouTube API missing ContentDetails part");
                if (videoFromYt.Snippet == null)
                    throw new Exception("Video from YouTube API missing Snippet part");
                if (videoFromYt.Snippet.Thumbnails == null)
                    throw new Exception("Video from YouTube API missing Snippet.Thumbnails part");
                if (videoFromYt.Statistics == null)
                    throw new Exception("Video from YouTube API missing Snippet part");
            }

            var videosFromYtMapped = vids.Select(v => Mapper.Map<YoutubeVideo>(v)).ToArray();
            var channels = videosFromYtMapped.Select(v => v.YouTubeChannel).DistinctBy(c => c.Id).ToArray();
            await Resolve<YouTubeRemoteService>().FetchChannelsPlaylistInfo(channels);
            return videosFromYtMapped;
        }

        public async Task Save(IEnumerable<YoutubeVideo> videos)
        {
            var channelsToInsert = await FilterToNotPersistedChannels(
                videos.Select(v => v.YouTubeChannel).DistinctBy(c => c.Id)
            );

            await Persist(ops =>
            {
                channelsToInsert.ForEach(ops.Add);
                videos.ForEach(vid =>
                {
                    ops.Add(
                        vid, 
                        v => v.YouTubeChannel = null, 
                        (original, copy) => original.Id = copy.Id
                    );
                });
            });
        }

        private async Task<IEnumerable<YouTubeChannel>> FilterToNotPersistedChannels(IEnumerable<YouTubeChannel> channels)
        {
            var allChannelsIdsFromDb = await Query<YouTubeChannel>().Select(c => c.Id).ToArrayAsync();
            var filtered = channels.Where(c => !c.Id.IsIn(allChannelsIdsFromDb));
            return filtered;
        }

        public async Task<IEnumerable<string>> FilterToUnknownVideosIds(IEnumerable<string> ids)
        {
            var foundIds = await Query<YoutubeVideo>()
                .Select(v => v.Id)
                .Where(vId => ids.Contains(vId))
                .ToArrayAsync();
            var notFoundIds = ids.Except(foundIds);
            return notFoundIds;
        }

        public async Task<YoutubeVideo[]> AddTracksToVideos(IEnumerable<string> ids)
        {
            var videos = await Query<YoutubeVideo>()
                .Where(v => ids.Contains(v.Id))
                .ToArrayAsync();
            foreach (var vid in videos)
                vid.Track = new Track();
            await Persist(ops => videos.ForEach(ops.Update));
            return videos;
        }

        public async Task SetVideoCategory(string videoId, YouTubeVideoCategory category)
        {
            var video = await Query<YoutubeVideo>().FirstOrDefaultAsync(v => v.Id == videoId);
            video.Category = category;
            await Persist(ops => ops.Update(video));
        }
    }
}
