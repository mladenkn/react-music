using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Music.App;

namespace Music.DevOps.Tasks
{
    public class GetChannelDetails : ServiceResolverAware
    {
        public GetChannelDetails(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public static void ConfigureCommand(CommandLineApplication c, IServiceProvider sp)
        {
            c.Command("channel-details", cmd =>
            {
                cmd.OnExecuteAsync(async _ =>
                {
                    
                });
            });
        }
    }
}
