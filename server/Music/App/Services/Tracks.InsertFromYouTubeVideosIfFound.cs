﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using Utilities;

namespace Music.App.Services
{
    public partial class TracksService
    {
        public async Task<Result> InsertFromYouTubeVideosIfFound(IReadOnlyCollection<string> wantedVideosIds)
        {
            var unknownVideosIds = (await FilterToUnknownVideosIds(wantedVideosIds)).ToArray();
            var videosFromYt = (await Resolve<YouTubeVideosRemoteService>().GetByIds(unknownVideosIds)).ToArray();

            var tracks = videosFromYt.Select(v => new Track { YoutubeVideos = new[] { v } }).ToArray();

            var channelsToInsert = await FilterToNotPersistedChannels(
                videosFromYt.Select(v => v.YouTubeChannel).DistinctBy(c => c.Id)
            );
            await Persist(ops =>
            {
                channelsToInsert.ForEach(ops.Add);
                tracks.ForEach(track =>
                {
                    ops.Add(track,
                        t =>
                        {
                            t.YoutubeVideos?.ForEach(v => v.YouTubeChannel = null);
                            t.TrackUserProps?.ForEach(tup =>
                            {
                                tup.YoutubeVideo = null;
                                if (tup.Track != null)
                                {
                                    tup.Track.YoutubeVideos = null;
                                }
                            });
                        },
                        (original, copy) => original.Id = copy.Id
                    );
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

        public async Task<IEnumerable<YouTubeChannel>> FilterToNotPersistedChannels(
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
