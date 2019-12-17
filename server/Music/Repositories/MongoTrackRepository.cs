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
            _collection = db.GetCollection<BsonDocument>("tracks2");
        }

        public async Task<IReadOnlyCollection<TrackUserProps>> GetCollection(GetTracksArguments args)
        {
            var tracksBson = await _collection
                .Find(t => true)
                .Skip(args.Skip)
                .Limit(args.Take)
                .ToListAsync();
            return MapToTrackUserPropsCollection(tracksBson);
        }

        public async Task<IReadOnlyCollection<TrackUserProps>> GetCollectionIfItemsExist(IEnumerable<string> ids)
        {
            var filter = Builders<BsonDocument>.Filter.In("ytID", ids);
            var tracksBson = await _collection.Find(filter).ToListAsync();
            return MapToTrackUserPropsCollection(tracksBson);
        }

        private IReadOnlyCollection<TrackUserProps> MapToTrackUserPropsCollection(IEnumerable<BsonDocument> tracksBson)
        {
            return tracksBson.Select(trackFromDb => new TrackUserProps
            {
                YtId = trackFromDb.GetValue("ytID").AsString,
                Year = ExtractYear(trackFromDb),
                Tags = trackFromDb.GetValue("tags").AsBsonArray.ToArray().Select(i => i.AsString),
                Genres = trackFromDb.GetValue("genres").AsBsonArray.ToArray().Select(i => i.AsString),
            }).ToList();
        }

        public async Task<int> Count() => (int) await _collection.CountDocumentsAsync(t => true);

        public async Task Save(IEnumerable<TrackUserProps> tracks)
        {
            foreach (var track in tracks)
            {
                var trackBson = new BsonDocument
                {
                    { "ytID", track.YtId },
                    { "tags", new BsonArray(track.Tags) },
                    { "genres", new BsonArray(track.Genres) },
                    { "year", track.Year },
                };

                var queryBuilder = Builders<BsonDocument>.Filter;
                var trackFilter = queryBuilder.Eq("ytID", track.YtId);

                var existingInstance = await _collection.Find(trackFilter).FirstOrDefaultAsync();
                if (existingInstance == null)
                    await _collection.InsertOneAsync(trackBson);
                else
                    await _collection.ReplaceOneAsync(trackFilter, trackBson);
            }
        }

        private int? ExtractYear(BsonDocument track)
        {
            if (!track.Contains("year"))
                return null;
            else
            {
                var yearBson = track.GetValue("year");
                if (yearBson.IsBsonNull)
                    return null;
                else if (yearBson.IsInt32)
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
