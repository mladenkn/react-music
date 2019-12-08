using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Music.Models;

namespace Music.Repositories
{
    public class TrackRepository
    {
        private readonly YoutubeVideoMasterRepository _videoRepo;

        public TrackRepository(YoutubeVideoMasterRepository videoRepo)
        {
            _videoRepo = videoRepo;
        }

        public async Task<GetTrackListResponse> GetList()
        {
            throw new NotImplementedException();
        }

        public async Task Save(Track t)
        {
            throw new NotImplementedException();
        }

        public async Task Update(Track t)
        {
            throw new NotImplementedException();
        }
    }

    public class GetTrackListResponse
    {
        public IEnumerable<Track> Data { get; set; }
        public int TotalCount { get; set; }
        public bool ThereIsMore { get; set; }
        public GetTrackListResponsePermissions Permissions { get; } = new GetTrackListResponsePermissions();
    }

    public class GetTrackListResponsePermissions
    {
        public bool CanEditTrackData { get; } = true;
        public bool CanFetchTrackRecommendations { get; } = true;
    }
}
