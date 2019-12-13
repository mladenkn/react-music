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

        public async Task Save(TrackUserProps trackUserProps)
        {
            await _repo.Save(trackUserProps);
        }
    }

    public class GetTrackListResponse : ListWithTotalCount<Track>
    {
        public GetTrackListResponsePermissions Permissions { get; } = new GetTrackListResponsePermissions();
    }

    public class GetTrackListResponsePermissions
    {
        public bool CanEditTrackData { get; } = true;
        public bool CanFetchTrackRecommendations { get; } = true;
    }
}