using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Music.Models;

namespace Music.Repositories
{
    public class TrackRepository
    {
        private readonly YoutubeVideoMasterRepository _videoRepo;
        private readonly MongoTrackRepository _mongoRepo;

        public TrackRepository(YoutubeVideoMasterRepository videoRepo, MongoTrackRepository mongoRepo)
        {
            _videoRepo = videoRepo;
            _mongoRepo = mongoRepo;
        }

        public async Task<ListWithTotalCount<Track>> GetCollection(GetTracksArguments args)
        {
            var allTracksFromDb = await _mongoRepo.GetCollection(args);
            var allTracksFromDbIds = allTracksFromDb.Select(t => t.Id).ToArray();
            var tracksFromYoutube = await _videoRepo.GetList(allTracksFromDbIds);

            var tracksFull = new List<Track>();
            foreach (var trackFromDb in allTracksFromDb)
            {
                var trackYtVideo = tracksFromYoutube.FirstOrDefault(tv => tv.Id == trackFromDb.Id);
                if(trackYtVideo != null)
                    tracksFull.Add(Create(trackYtVideo, trackFromDb));
            }

            return new ListWithTotalCount<Track>
            {
                Data = tracksFull,
                TotalCount = await _mongoRepo.Count(),
            };
        }

        private static Track Create(YoutubeVideo fromYt, TrackUserProps fromDb) =>
            new Track
            {
                YtId = fromYt.Id,
                Title = fromYt.Title,
                Image = fromYt.Image,
                Description = fromYt.Description,
                Channel = new TrackChannel
                {
                    Id = fromYt.ChannelId,
                    Title = fromYt.ChannelTitle,
                },
                Year = fromDb.Year,
                Genres = fromDb.Genres,
                Tags = fromDb.Tags
            };

        public async Task Save(TrackUserProps t)
        {
            await _mongoRepo.Save(t);
        }
    }

    public class GetTracksArguments
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }

    
}
