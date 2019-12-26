using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kernel
{
    public abstract class ServiceResolverAware<TDbContext>
        where TDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        
        protected TDbContext Db { get; }

        protected IMapper Mapper { get; }

        protected ServiceResolverAware(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Db = _serviceProvider.GetService<TDbContext>();
            Mapper = _serviceProvider.GetService<IMapper>();
        }

        protected TService Resolve<TService>() => _serviceProvider.GetService<TService>();
    }
}
