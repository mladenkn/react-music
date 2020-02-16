using System;
using Kernel;
using Music.DataAccess;

namespace Music
{
    public class RequestExecutor : RequestExecutor<MusicDbContext>
    {
        public RequestExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }

    public class ServiceResolverAware : ServiceResolverAware<MusicDbContext>
    {
        public ServiceResolverAware(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
