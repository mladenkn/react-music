using System;
using Kernel;

namespace Music.App
{

    public class ServiceResolverAware : ServiceResolverAware<MusicDbContext>
    {
        public ServiceResolverAware(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
