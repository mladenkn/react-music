using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using Utilities;

namespace Music.App.Services
{
    public class InsertTracksFromYouTubeVideosIfFound : ServiceResolverAware
    {
        public InsertTracksFromYouTubeVideosIfFound(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<Result> Execute(IReadOnlyCollection<string> wantedVideosIds)
        {
            var unknownVideosIds = (await FilterToUnknownVideosIds(wantedVideosIds)).ToArray();
            var videosFromYt = (await Resolve<YouTubeVideoServices>().GetByIds(unknownVideosIds)).ToArray();
            
            var tracks = videosFromYt.Select(v => new Track { YoutubeVideos = new[] {v} }).ToArray();

            var channelsToInsert = await FilterToNotPersistedChannels(
                videosFromYt.Select(v => v.YouTubeChannel).DistinctBy(c => c.Id)
            );
            await Persist(ops =>
            {
                ops.InsertYouTubeChannels(channelsToInsert);
                ops.InsertTracks(tracks, t =>
                {
                    if (t.YoutubeVideos != null) 
                        t.YoutubeVideos.ForEach(v => v.YouTubeChannel = null);
                    if(t.TrackUserProps != null)
                        t.TrackUserProps.ForEach(tup =>
                        {
                            tup.YoutubeVideo = null;
                            if (tup.Track != null)
                            {
                                tup.Track.YoutubeVideos = null;
                            }
                        });
                });
            });

            var notFoundVideosIds = wantedVideosIds.Except(videosFromYt.Select(v => v.Id));

            return new Result
            {
                NotFoundVideoIds = notFoundVideosIds,
                NewTracks = tracks,
                NewYouTubeVideos = videosFromYt
            };
        }

        private async Task<IEnumerable<string>> FilterToUnknownVideosIds(IEnumerable<string> ids)
        {
            var foundIds = await Query<YoutubeVideo>()
                .Select(v => v.Id)
                .Where(vId => ids.Contains(vId))
                .ToArrayAsync();
            var notFoundIds = ids.Except(foundIds);
            return notFoundIds;
        }

        private async Task<IEnumerable<YouTubeChannel>> FilterToNotPersistedChannels(
            IEnumerable<YouTubeChannel> channels)
        {
            var allChannelsIdsFromDb = await Query<YouTubeChannel>().Select(c => c.Id).ToArrayAsync();
            var filtered = channels.Where(c => !c.Id.IsIn(allChannelsIdsFromDb));
            return filtered;
        }

        public class Result
        {
            public IEnumerable<string> NotFoundVideoIds { get; set; }
            public IReadOnlyCollection<Track> NewTracks { get; set; }
            public IReadOnlyCollection<YoutubeVideo> NewYouTubeVideos { get; set; }
        }
    }
}
