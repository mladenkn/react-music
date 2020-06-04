using Music.App.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;

namespace Music.Admin.Services
{
    public class AdminCommandExecutor : ServiceResolverAware
    {
        public AdminCommandExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<string> ExecuteCommand(string commandYaml)
        {
            var yamlService = Resolve<YamlService>();
            var cmd = yamlService.DeserializeToDictionary(commandYaml);
            var type = cmd.GetValueOrDefault("type");

            async Task<object> Execute()
            {
                var ytService = Resolve<YouTubeRemoteService>();

                switch (type)
                {
                    case "GetChannelDetails":
                    {
                        var channelId = cmd.Get<string>("channelId");
                        return await ytService.GetChannelDetails(channelId);
                    }
                    case "GetChannelsOfUser":
                    {
                        var username = cmd.Get<string>("username");
                        return await ytService.GetChannelsOfUser(username);
                    }
                    case "SaveChannelWithVideosToTempStorage":
                    {
                        var channelId = cmd.Get<string>("channelId");
                        await Resolve<ChannelsWithVideosTempStorage>().ToTemp(channelId);
                        return "Channel videos saved.";
                    }
                    case "SaveChannelWithVideosFromTempToDb":
                    {
                        var fileName = cmd.Get<string>("file");
                        await Resolve<ChannelsWithVideosTempStorage>().FromTempToDb(fileName);
                        return "Channel videos saved.";
                    }
                    case "GetVideosWithoutTracks":
                    {
                        return await Resolve<YouTubeVideosService>().GetVideosWithoutTracks();
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
                    return yamlService.Serialize(r);
                }
                catch (Exception)
                {
                    throw new ApplicationException("Command failed to execute probably because of user's mistake.");
                }
            }
        }
    }
}
