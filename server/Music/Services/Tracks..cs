using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.DbModels;
using Music.Models;
using Utilities;

namespace Music.Services
{
    public partial class TracksService
    {
        public async Task<IReadOnlyCollection<TrackForHomeSection>> GetTracksWithVideoIdsIfFound(IEnumerable<string> wantedTracksYtIds)
        {
            var curUserId = Resolve<ICurrentUserContext>().Id;
            var tracks = await Query<Track>()
                .Where(track => wantedTracksYtIds.Contains(track.YoutubeVideos.First().Id))
                .Select(TrackForHomeSection.FromTrack(curUserId))
                .ToArrayAsync();
            return tracks;
        }

        public async Task<IEnumerable<Track>> SaveTracksFromVideos(IReadOnlyCollection<YoutubeVideo> videos)
        {
            // Sa Persist error da pokušavan update-at isti video više puta
            foreach (var video in videos)
            {
                video.Track = new Track();
                var videoCopy = ReflectionUtils.ShallowCopy(video);
                videoCopy.YouTubeChannel = null;
                Db.Update(videoCopy);
            }
            await Db.SaveChangesAsync();

            return videos.Select(v => v.Track);
        }

        public async Task Save(IReadOnlyCollection<SaveTrackModel> tracks)
        {
            var videosIds = tracks.Select(t => t.YouTubeVideoId).ToArray();

            var youTubeVideosService = Resolve<YouTubeVideosService>();
            await youTubeVideosService.EnsureAreSavedIfFound(videosIds);

            var videos = await youTubeVideosService.Get(videosIds);

            foreach (var youtubeVideo in videos)
            {
                var saveTrackModel = tracks.Single(t => t.YouTubeVideoId == youtubeVideo.Id);
                youtubeVideo.Track = new Track
                {
                    TrackUserProps = new[]
                    {
                        new TrackUserProps
                        {
                            UserId = 1,
                            InsertedAt = DateTime.Now,
                            TrackTags = saveTrackModel.Tags.Select(tag => new TrackUserPropsTag { Value = tag } ).ToArray(),
                            Year = saveTrackModel.Year,
                            YoutubeVideoId = saveTrackModel.YouTubeVideoId
                        }
                    }
                };
            }

            // Sa Persist error da pokušavan update-at isti video više puta
            videos.ForEach(v => Db.Update(v));
            await Db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Track>> GetTracksWithoutYouTubeVideos()
        {
            return await Query<Track>()
                .Include(t => t.YoutubeVideos)
                .Where(t => t.YoutubeVideos.Count == 0)
                .ToArrayAsync();
        }

        public async Task Delete(IReadOnlyCollection<long> ids)
        {
            var tracks = await Query<Track>()
                .Where(t => ids.Contains(t.Id))
                .ToArrayAsync();
            if (tracks.Length != ids.Count)
                throw new Exception("Not all tracks were found.");
            await Persist(ops => tracks.ForEach(ops.Remove));
        }
    }
}
