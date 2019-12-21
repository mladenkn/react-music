using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music.Models;

namespace Music.Repositories
{
    public class YoutubeVideoMasterRepository
    {
        private readonly YoutubeDataApiVideoRepository _remoteRepo;
        private readonly YoutubeVideoMongoRepository _localRepo;

        public YoutubeVideoMasterRepository(YoutubeDataApiVideoRepository remoteRepo,
            YoutubeVideoMongoRepository localRepo)
        {
            _remoteRepo = remoteRepo;
            _localRepo = localRepo;
        }

        public async Task<IEnumerable<YoutubeVideo>> GetList(IReadOnlyCollection<string> ids)
        {
            var (videosSavedLocally, notFoundVideosIds) = await _localRepo.GetList(ids);
            if (notFoundVideosIds.Count() == 0)
                return videosSavedLocally;
            else
            {
                var remotelyFetchedVideos = await _remoteRepo.GetList(notFoundVideosIds);
                await _localRepo.Save(remotelyFetchedVideos);
                return videosSavedLocally.Concat(remotelyFetchedVideos);
            }
        }

        public Task<IReadOnlyCollection<YoutubeVideoFromSearchResults>> Search(YoutubeTrackQuery query) => _remoteRepo.Search(query);
    }
    
    public class YoutubeTrackQuery
    {
        public string SearchQuery { get; set; }
        public string ChannelTitle { get; set; }
        public int MaxResults { get; set; }
    }
}
