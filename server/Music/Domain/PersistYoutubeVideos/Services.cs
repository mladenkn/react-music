using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Music.Domain.PersistYoutubeVideos
{
    public delegate Task<IReadOnlyCollection<Video>> ListYoutubeVideos(IEnumerable<string> parts, IEnumerable<string> ids);

    public class PersistYoutubeVideosServices : ServiceResolverAware
    {
        public PersistYoutubeVideosServices(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IReadOnlyCollection<Video>> ListYoutubeVideos(IEnumerable<string> parts, IEnumerable<string> ids)
        {
            var partsAsOneString = string.Join(",", parts);
            var idsAsOneString = string.Join(",", ids);
            var ytService = Resolve<YouTubeService>();
            var request = ytService.Videos.List(partsAsOneString);
            request.Id = idsAsOneString;
            var result = await request.ExecuteAsync();
            return result.Items.ToList();
        }
    }
}
