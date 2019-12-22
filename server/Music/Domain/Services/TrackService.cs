using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Music.Domain.Models;

namespace Music.Domain.Services
{
    public class TrackService
    {

        public async Task<GetTrackListResponse> GetList(GetTracksArguments args)
        {
            throw new NotImplementedException();

        }

        public async Task<SearchYoutubeResult> SearchYoutube(YoutubeTrackQuery query)
        {
            throw new NotImplementedException();

        }

        public async Task<TrackPermissions> Save(IEnumerable<TrackUserProps> tracks)
        {
            throw new NotImplementedException();
        }
    }

    public class GetTrackListResponse : ListWithTotalCount<Track>
    {
        public TrackPermissions Permissions { get; } = new TrackPermissions();
    }

    public class SearchYoutubeResult
    {
        public IEnumerable<Track> Tracks { get; set; }
        public TrackPermissions Permissions { get; } = new TrackPermissions();
    }

    public class GetTracksArguments
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }

    public class YoutubeTrackQuery
    {
        public string SearchQuery { get; set; }
        public string ChannelTitle { get; set; }
        public int MaxResults { get; set; }
    }
}