using System;
using Kernel;
using Music.Domain.Shared;

namespace Music
{

    public class ServiceResolverAware : ServiceResolverAware<MusicDbContext>
    {
        public ServiceResolverAware(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
