using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Music.Models;

namespace Music.Repositories
{
    public class YoutubeVideoRepository
    {
        private readonly IMongoCollection<BsonDocument> _col;

        public YoutubeVideoRepository(IMongoDatabase db)
        {
            _col = db.GetCollection<BsonDocument>("youtubeVideos");
        }

        public async Task<YoutubeVideo> GetList(string[] ids)
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

        public async Task Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
