using System;
using Kernel;
using Music.DataAccess;

namespace Music
{

    public class ServiceResolverAware : ServiceResolverAware<MusicDbContext>
    {
        public ServiceResolverAware(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
