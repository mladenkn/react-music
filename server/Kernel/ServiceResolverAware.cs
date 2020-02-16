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
            Db = _serviceProvider.GetRequiredService<TDbContext>();
            Mapper = _serviceProvider.GetRequiredService<IMapper>();
        }

        protected TService Resolve<TService>() => _serviceProvider.GetRequiredService<TService>();
    }

    public abstract class RequestExecutor<TDbContext> : ServiceResolverAware<TDbContext> where TDbContext : DbContext
    {
        protected RequestExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
