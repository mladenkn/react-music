using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using Utilities;

namespace Music.App
{
    public class DataPersistor : ServiceResolverAware
    {
        private readonly MusicDbContext _db;

        public DataPersistor(MusicDbContext db, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _db = db;
        }

        public async Task InsertTracks(IReadOnlyCollection<Track> tracks)
        {
            var allChannels = new List<YouTubeChannel>();

            var tracksReadyToInsert = tracks.Select(track =>
            {
                var copy = ReflectionUtils.ShallowCopy(track);
                var ytVideo = copy.YoutubeVideos.First();
                
                if(allChannels.All(c => c.Id != ytVideo.YoutubeChannelId))
                    allChannels.Add(ytVideo.YouTubeChannel);

                ytVideo.YouTubeChannel = null;
                return copy;
            });

            var allChannelsIdsFromDb = await _db.Set<YouTubeChannel>().Select(c => c.Id).ToArrayAsync();
            var channelsReadyToInsert = allChannels.Where(c => !c.Id.IsIn(allChannelsIdsFromDb));

            _db.Tracks.AddRange(tracksReadyToInsert);
            _db.YouTubeChannels.AddRange(channelsReadyToInsert);

            await _db.SaveChangesAsync();
        }
    }
}
