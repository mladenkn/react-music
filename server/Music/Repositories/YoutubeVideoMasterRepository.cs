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
        private readonly YoutubeHtmlScrapperVideoRepository _youtubeHtmlScrapperVideoRepository;

        public YoutubeVideoMasterRepository(YoutubeDataApiVideoRepository remoteRepo,
            YoutubeVideoMongoRepository localRepo, YoutubeHtmlScrapperVideoRepository youtubeHtmlScrapperVideoRepository)
        {
            _remoteRepo = remoteRepo;
            _localRepo = localRepo;
            _youtubeHtmlScrapperVideoRepository = youtubeHtmlScrapperVideoRepository;
        }

        public async Task<IEnumerable<YoutubeVideo>> GetList(
            IReadOnlyCollection<string> ids, Func<IEnumerable<string>, 
            IEnumerable<string>> prepareNotFoundIdsForSearch = null
            )
        {
            var (videosSavedLocally, notFoundVideosIds) = await _localRepo.GetList(ids);
            if (notFoundVideosIds.Count == 0)
                return videosSavedLocally;
            else
            {
                var notFoundIdsForFetch = prepareNotFoundIdsForSearch == null ? 
                    notFoundVideosIds : 
                    prepareNotFoundIdsForSearch(notFoundVideosIds).ToArray();
                var remotelyFetchedVideos = await _remoteRepo.GetList(notFoundIdsForFetch);
                await _localRepo.Save(remotelyFetchedVideos);
                return videosSavedLocally.Concat(remotelyFetchedVideos);
            }
        }

        public async Task<IReadOnlyCollection<YoutubeVideo>> Search(YoutubeTrackQuery query)
        {
            var vidIds = await _youtubeHtmlScrapperVideoRepository.SearchIds(query.SearchQuery);
            var videos = await GetList(
                vidIds.ToArray(), 
                notFoundIds => notFoundIds.Take(50)
            );
            return videos.ToArray();
        }
    }
    
    public class YoutubeTrackQuery
    {
        public string SearchQuery { get; set; }
        public string ChannelTitle { get; set; }
        public int MaxResults { get; set; }
    }
}
