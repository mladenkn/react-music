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
            if (type == null)
                throw new ApplicationException();
            else
            {
                var ytService = Resolve<YouTubeRemoteService>();
                try
                {
                    switch (type)
                    {
                        case "GetChannelDetails":
                        {
                            var channelId = cmd.Get<string>("channelId");
                            var response = await ytService.GetChannelDetails(channelId);
                            return yamlService.Serialize(response);
                        }
                        case "GetChannelsOfUser":
                        {
                            var username = cmd.Get<string>("username");
                            var response = await ytService.GetChannelsOfUser(username);
                            return yamlService.Serialize(response);
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
                        default:
                            return "Unsupported command";
                    }
                }
                catch (Exception)
                {
                    throw new ApplicationException("Command failed to execute probably because of user's mistake.");
                }
            }
        }
    }
}
