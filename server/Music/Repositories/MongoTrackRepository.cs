using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Music.Repositories
{
    public class MongoTrackRepository
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoTrackRepository(IMongoDatabase db)
        {
            _collection = db.GetCollection<BsonDocument>("tracks");
        }

        public async Task<IReadOnlyCollection<TrackUserProps>> GetAll()
        {
            var trackCursor = await _collection.FindAsync(t => true);
            var allTracks = (await trackCursor.ToListAsync()).Take(50);

            return allTracks.Select(trackFromDb => new TrackUserProps
            {
                Id = trackFromDb.GetValue("ytID").AsString,
                Year = ExtractYear(trackFromDb),
                Tags = trackFromDb.GetValue("tags").AsBsonArray.ToArray().Select(i => i.AsString),
                Genres = trackFromDb.GetValue("genres").AsBsonArray.ToArray().Select(i => i.AsString),
            }).ToList();
        }

        private int? ExtractYear(BsonDocument track)
        {
            if (!track.ContainsValue("year"))
                return null;
            else
            {
                var yearBson = track.GetValue("year");
                if (yearBson.IsInt32)
                    return yearBson.AsInt32;
                else if (yearBson.IsString)
                    return int.Parse(yearBson.AsString);
                else
                {
                    throw new Exception("Unsupported type for Track.Year");
                }
            }
        }
    }

    public class TrackUserProps
    {
        public string Id { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public int? Year { get; set; }
    }
}
