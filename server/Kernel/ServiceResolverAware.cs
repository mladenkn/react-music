using System;
using System.Linq;
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

        protected IQueryable<T> Query<T>() where T : class, IDatabaseEntity => Db.Set<T>();
    }
}
