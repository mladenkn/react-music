using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Music.Models;

namespace Music.Repositories
{
    public class YoutubeVideoMongoRepository
    {
        private readonly IMongoCollection<YoutubeVideo> _col;

        public YoutubeVideoMongoRepository(IMongoDatabase db)
        {
            _col = db.GetCollection<YoutubeVideo>("youtubeVideos");
        }

        public async Task<IEnumerable<string>> GetAllIds()
        {
            var props = Builders<YoutubeVideo>.Projection.Include(v => v.Id);
            var vids = await _col.Find(v => true).Project(props).ToListAsync();
            var ids = vids.Select(v => v.GetElement("_id").Value.AsString);
            return ids;
        }

        public async Task<(IEnumerable<YoutubeVideo> videos, IReadOnlyCollection<string> notFoundIds)>
            GetList(IEnumerable<string> wantedVideosIds)
        {
            var foundVideos = await _col.Find(v => wantedVideosIds.Contains(v.Id)).ToListAsync();
            var notFoundIds = wantedVideosIds.Except(foundVideos.Select(v => v.Id)).ToArray();
            return (foundVideos, notFoundIds);
        }

        public async Task Save(IReadOnlyCollection<YoutubeVideo> vids)
        {
            if(vids.Count > 0)
                await _col.InsertManyAsync(vids);
        }

        public async Task Update(YoutubeVideo v)
        {
            throw new NotImplementedException();
        }
    }
}
