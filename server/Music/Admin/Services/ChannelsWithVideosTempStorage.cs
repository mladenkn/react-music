using System;
using System.Threading.Tasks;

namespace Music.Admin.Services
{
    public class ChannelsWithVideosTempStorage : ServiceResolverAware
    {
        public ChannelsWithVideosTempStorage(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        
        public async Task PersistToFile((string userId, string channelId) args)
        {

        }
    }
}
