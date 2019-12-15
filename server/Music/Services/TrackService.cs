using System.Collections.Generic;
using System.Threading.Tasks;
using Music.Models;
using Music.Repositories;

namespace Music.Services
{
    public class TrackService
    {
        private readonly TrackRepository _repo;

        public TrackService(TrackRepository repo)
        {
            _repo = repo;
        }

        public async Task<GetTrackListResponse> GetList(GetTracksArguments args)
        {
            var r = await _repo.GetCollection(args);
            return new GetTrackListResponse
            {
                Data = r.Data,
                TotalCount = r.TotalCount,
            };
        }

        public async Task<TrackPermissions> Save(IEnumerable<TrackUserProps> tracks)
        {
            await _repo.Save(tracks);
            return new TrackPermissions();
        }
    }

    public class GetTrackListResponse : ListWithTotalCount<Track>
    {
        public TrackPermissions Permissions { get; } = new TrackPermissions();
    }
}