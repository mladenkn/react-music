using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Music.Services;

namespace Music.Operations
{
    public class MigrationToRelational
    {
        private readonly IYoutubeVideoService _youtubeVideoService;
        private readonly IMongoCollection<BsonDocument> _tracksCollection;

        public MigrationToRelational(IMongoDatabase db, IYoutubeVideoService youtubeVideoService)
        {
            _youtubeVideoService = youtubeVideoService;
            _tracksCollection = db.GetCollection<BsonDocument>("tracks");
        }

        public async Task Run()
        {
            var trackCursor = await _tracksCollection.FindAsync(t => true);
            var allTracks = await trackCursor.ToListAsync();
            var allTrackIds = allTracks.Select(t => (string)t.GetValue("ytID")).ToList();
            var allTracksFromYt = await _youtubeVideoService.GetList(allTrackIds);
        }
    }
}
