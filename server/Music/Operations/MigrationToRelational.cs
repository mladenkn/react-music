using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Music.Services;

namespace Music.Operations
{
    public class MigrationToRelational
    {
        private readonly YoutubeDataApiVideoService _youtubeTrackService;
        private readonly IMongoCollection<BsonDocument> _tracksCollection;

        public MigrationToRelational(IMongoDatabase db, YoutubeDataApiVideoService youtubeTrackService)
        {
            _youtubeTrackService = youtubeTrackService;
            _tracksCollection = db.GetCollection<BsonDocument>("tracks");
        }

        public async Task Run()
        {
            var trackCursor = await _tracksCollection.FindAsync(t => true);
            var allTracks = await trackCursor.ToListAsync();
            var allTrackIds = allTracks.Select(t => (string)t.GetValue("ytID")).ToList();
            var allTracksFromYt = await _youtubeTrackService.GetList(allTrackIds);
        }
    }
}
