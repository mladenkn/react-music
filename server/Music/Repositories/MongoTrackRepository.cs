using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Music.Models;

namespace Music.Repositories
{
    public class MongoTrackRepository
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoTrackRepository(IMongoDatabase db)
        {
            _collection = db.GetCollection<BsonDocument>("tracks");
        }

        public async Task<IReadOnlyCollection<TrackUserProps>> GetCollection(GetTracksArguments args)
        {
            var allTracks = await _collection
                .Find(t => true)
                .Skip(args.Skip)
                .Limit(args.Take)
                .ToListAsync();

            return allTracks.Select(trackFromDb => new TrackUserProps
            {
                Id = trackFromDb.GetValue("ytID").AsString,
                Year = ExtractYear(trackFromDb),
                Tags = trackFromDb.GetValue("tags").AsBsonArray.ToArray().Select(i => i.AsString),
                Genres = trackFromDb.GetValue("genres").AsBsonArray.ToArray().Select(i => i.AsString),
            }).ToList();
        }

        public async Task<int> Count() => (int) await _collection.CountDocumentsAsync(t => true);

        public async Task Save(TrackUserProps track)
        {

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
}
