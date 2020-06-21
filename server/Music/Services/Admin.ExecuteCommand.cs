using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Music.Services
{
    public partial class AdminService
    {
        public async Task<object> ExecuteCommand(JObject cmd)
        {
            var type = cmd.GetValue("type")!.Value<string>();

            async Task<object> Execute()
            {
                var ytService = Resolve<YouTubeRemoteService>();

                switch (type)
                {
                    case "GetChannelDetails":
                    {
                        var channelId = cmd.GetValue("channelId")!.Value<string>();
                        var ensureChannelsAreSaved = cmd.GetValue("ensureChannelsAreSaved")?.Value<bool>() ?? true;
                        return await ytService.GetChannelDetails(channelId, ensureChannelsAreSaved);
                    }
                    case "GetChannelsOfUser":
                    {
                        var username = cmd.GetValue("username")!.Value<string>();
                        var ensureChannelsAreSaved = cmd.GetValue("ensureChannelsAreSaved")?.Value<bool>() ?? true;
                        return await ytService.GetChannelsOfUser(username, ensureChannelsAreSaved);
                    }
                    case "GetYouTubeVideosWithoutTracks":
                        return await Resolve<YouTubeVideosService>().GetVideosWithoutTracks();

                    case "GetTracksWithoutYouTubeVideos":
                        return await Resolve<TracksService>().GetTracksWithoutYouTubeVideos();

                    case "AddTracksToYouTubeVideos":
                    {
                        var videoIds = cmd.GetValue("videoIds")!.Values<string>();
                        return await Resolve<YouTubeVideosService>().AddTracksToVideos(videoIds);
                    }

                    case "DeleteTracks":
                    {
                        var trackIds = cmd.GetValue("tracks")!.Values<long>().ToArray();
                        await Resolve<TracksService>().Delete(trackIds);
                        return "Successfully deleted all stated tracks";
                    }
                    case "GetVideosOfChannelFromYouTubeApi":
                    {
                        var channelId = cmd.GetValue("channelId")!.Value<string>();
                        var maxResults = cmd.GetValue("maxResults")!.Value<int>();
                        var parts = cmd.GetValue("parts")!.Values<string>();
                        var videos = await Resolve<YouTubeRemoteService>().GetVideosOfChannel(channelId, parts, maxResults);
                        return videos;
                    }
                    case "GetCommands":
                    {
                        return await Resolve<AdminService>().GetCommands();
                    }
                    case "GetKnownYouTubeChannels":
                        return await Resolve<YouTubeChannelService>().Get();

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
