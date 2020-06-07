using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Music.Services
{
    public class AdminCommandExecutor : ServiceResolverAware
    {
        public AdminCommandExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<object> ExecuteCommand(JsonElement cmd)
        {
            var type = cmd.GetProperty("type").GetString();

            async Task<object> Execute()
            {
                var ytService = Resolve<YouTubeRemoteService>();

                switch (type)
                {
                    case "GetChannelDetails":
                    {
                        var channelId = cmd.GetProperty("channelId").GetString();
                        return await ytService.GetChannelDetails(channelId);
                    }
                    case "GetChannelsOfUser":
                    {
                        var username = cmd.GetProperty("username").GetString();
                        return await ytService.GetChannelsOfUser(username);
                    }
                    case "GetYouTubeVideosWithoutTracks":
                        return await Resolve<YouTubeVideosService>().GetVideosWithoutTracks();

                    case "GetTracksWithoutYouTubeVideos":
                        return await Resolve<TracksService>().GetTracksWithoutYouTubeVideos();

                    case "DeleteTracks":
                    {
                        var trackIds = cmd.GetProperty("tracks").EnumerateArray().Select(e => e.GetInt64()).ToArray();
                        await Resolve<TracksService>().Delete(trackIds);
                        return "Successfully deleted all stated tracks";
                    }
                    case "GetVideosOfChannelFromYouTubeApi":
                    {
                        var channelId = cmd.GetProperty("channelId").GetString();
                        var maxResults = cmd.GetProperty("maxResults").GetInt32();
                        var parts = cmd.GetProperty("parts").EnumerateArray().Select(p => p.GetString());
                        var videos = await Resolve<YouTubeRemoteService>().GetVideosOfChannel(channelId, parts, maxResults);
                        return videos;
                    }
                    default:
                        return "Unsupported command";
                }
            }

            if (type == null)
                throw new ApplicationException();
            else
            {
                try
                {
                    var r = await Execute();
                    return r;
                }
                catch (ApplicationException e)
                {
                    throw new ApplicationException($"Command failed to execute because of user's mistake. {e.Message}");
                }
            }
        }
    }
}
