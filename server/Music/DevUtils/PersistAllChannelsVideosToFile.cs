using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App;
using Music.App.YouTubeVideos;

namespace Music.DevUtils
{
    public class PersistAllChannelsVideosToFile : ServiceResolverAware
    {
        public PersistAllChannelsVideosToFile(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Execute()
        {
            var allChannels = await Db.YouTubeChannels.ToArrayAsync();
            var playlistsIds = allChannels.Select(c => c.UploadsPlaylistId);
            var playlistsVideos = await Resolve<YouTubeVideoService>().GetAllFromPlaylist(playlistsIds.First());
        }
    }
}
