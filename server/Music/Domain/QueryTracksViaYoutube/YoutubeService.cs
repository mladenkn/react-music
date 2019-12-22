using Google.Apis.YouTube.v3;

namespace Music.Domain.QueryTracksViaYoutube
{
    public interface IYoutubeService
    {
        VideosResource.ListRequest ListVideos(string part);
    }

    public class YouTubeService : IYoutubeService
    {
        private readonly Google.Apis.YouTube.v3.YouTubeService _wrappedService;

        public YouTubeService(Google.Apis.YouTube.v3.YouTubeService wrappedService)
        {
            _wrappedService = wrappedService;
        }

        public VideosResource.ListRequest ListVideos(string part)
        {
            return _wrappedService.Videos.List(part);
        }
    }
}
