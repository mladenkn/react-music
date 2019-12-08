using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Music.Models;

namespace Music.Repositories
{
    public class YoutubeVideoMongoRepository
    {
        private readonly IMongoCollection<BsonDocument> _col;

        public YoutubeVideoMongoRepository(IMongoDatabase db)
        {
            _col = db.GetCollection<BsonDocument>("youtubeVideos");
        }

        public async Task<(IEnumerable<YoutubeVideo> videos, IEnumerable<string> notFoundIds)>
            GetList(IEnumerable<string> ids)
        {
            throw new NotImplementedException();
        }

        public async Task Save(YoutubeVideo v)
        {
            throw new NotImplementedException();
        }

        public async Task Update(YoutubeVideo v)
        {
            throw new NotImplementedException();
        }
    }
}
