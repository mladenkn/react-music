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
            var allTracksFromDbIds = allTracksFromDb.Select(t => t.YtId).ToArray();
            var tracksFromYoutube = await _videoRepo.GetList(allTracksFromDbIds);

            var tracksFull = new List<Track>();
            foreach (var trackFromDb in allTracksFromDb)
            {
                var trackYtVideo = tracksFromYoutube.FirstOrDefault(tv => tv.Id == trackFromDb.YtId);
                if(trackYtVideo != null)
                    tracksFull.Add(Create(trackYtVideo, trackFromDb));
            }

            return new ListWithTotalCount<Track>
            {
                Data = tracksFull,
                TotalCount = await _mongoRepo.Count(),
            };
        }
        public async Task<IEnumerable<Track>> GetCollectionFromYoutubeSearch(YoutubeTrackQuery query)
        {
            var videos = await _videoRepo.Search(query);
            var tracksUserProps = await _mongoRepo.GetCollectionIfItemsExist(videos.Select(v => v.Id));
            var tracks = videos.Select(video =>
            {
                var trackUserProps = tracksUserProps.FirstOrDefault(t => t.YtId == video.Id);
                return Create(video, trackUserProps);
            });
            return tracks;
        }

        private static Track Create(YoutubeVideo fromYt, TrackUserProps usersProps)
        {
            var hasUsersProps = usersProps != null;
            return new Track
            {
                YtId = fromYt.Id,
                Title = fromYt.Title,
                Image = fromYt.GetImage(),
                Description = fromYt.Description,
                Channel = new TrackChannel
                {
                    Id = fromYt.ChannelId,
                    Title = fromYt.ChannelTitle,
                },
                Year = hasUsersProps ? usersProps.Year : null,
                Genres = hasUsersProps ? usersProps.Genres : new string[0],
                Tags = hasUsersProps ? usersProps.Tags : new string[0]
            };
        }

        public Task Save(IEnumerable<TrackUserProps> tracks) => _mongoRepo.Save(tracks);
    }

    public class GetTracksArguments
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
