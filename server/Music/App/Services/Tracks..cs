using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using Music.App.Models;
using Utilities;

namespace Music.App.Services
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

        public async Task<IEnumerable<Track>> SaveTracksFromVideos(IEnumerable<YoutubeVideo> videos)
        {
            var pairs = videos.Select(v => (track: new Track(), video: v)).ToArray();
            
            await Persist(ops => pairs.ForEach(p => ops.Add(p.track)));

            foreach(var (track, video) in pairs)
                video.TrackId = track.Id;

            // try to use Persist
            foreach (var (track, video) in pairs)
            {
                var videoCopy = ReflectionUtils.ShallowCopy(video);
                videoCopy.YouTubeChannel = null;
                Db.Update(videoCopy);
            }
            await Db.SaveChangesAsync();
            
            //await Persist(ops =>
            //{
            //    pairs.ForEach(p =>
            //    {
            //        ops.Update(p.video, v => v.YouTubeChannel = null);
            //    });
            //});

            return pairs.Select(p => p.track);
        }

        public async Task<IEnumerable<Track>> GetTracksWithoutYouTubeVideos()
        {
            return await Query<Track>()
                .Include(t => t.YoutubeVideos)
                .Where(t => t.YoutubeVideos.Count() == 0)
                .ToArrayAsync();
        }

        public async Task Delete(IEnumerable<long> ids)
        {
            var trakcs = await Query<Track>()
                .Where(t => ids.Contains(t.Id))
                .ToArrayAsync();
            await Persist(ops => trakcs.ForEach(ops.Remove));
        }
    }
}
