using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Models;
using Utilities;

namespace Music.DataAccess
{
    public class DataPersistor
    {
        private readonly MusicDbContext _db;

        public DataPersistor(MusicDbContext db)
        {
            _db = db;
        }

        public async Task InsertYoutubeVideos(IEnumerable<YoutubeVideo> videos)
        {
            var videosReadyToInsert = videos.Select(v =>
            {
                var copy = ReflectionUtils.ShallowCopy(v);
                copy.YoutubeChannel = null;
                return copy;
            });

            var allChannels = videos.Select(v => v.YoutubeChannel).DistinctBy(c => c.Id);
            var allChannelsIdsFromDb = await _db.Set<YoutubeChannel>().Select(c => c.Id).ToArrayAsync();
            var channelsReadyToInsert = allChannels.Where(c => !c.Id.IsIn(allChannelsIdsFromDb));

            _db.AddRange(videosReadyToInsert);
            _db.AddRange(channelsReadyToInsert);

            await _db.SaveChangesAsync();
        }
    }
}
