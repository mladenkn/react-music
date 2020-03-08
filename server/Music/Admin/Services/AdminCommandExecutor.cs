using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Music.Admin.Services
{
    public class AdminCommandExecutor : ServiceResolverAware
    {
        public AdminCommandExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<string> ExecuteComamnd(string commandYaml)
        {
            var yamlService = Resolve<YamlService>();
            var cmd = yamlService.DeserializeToDictionary(commandYaml);
            var type = cmd.GetValueOrDefault("type");
            if (type == null)
                throw new ApplicationException();
            else
            {
                switch (type)
                {
                    case "GetChannelDetails":
                        var channelId = cmd["channelId"];
                        var response = new Dictionary<string, object>
                        {
                            ["channelId"] = "qojrt'1u45iowfhnawiofh",
                            ["channelTitle"] = "Mate i Jure",
                            ["videosCount"] = 4235
                        };
                        return yamlService.Serialize(response);

                    default:
                        throw new ApplicationException("Unsupported command type");
                }
            }
        }
    }
}
