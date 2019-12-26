using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kernel
{
    public abstract class ServiceResolverAware
    {
        private readonly IServiceProvider _serviceProvider;
        
        protected IDatabase Db { get; }

        protected IMapper Mapper { get; }

        protected ServiceResolverAware(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Db = _serviceProvider.GetService<IDatabase>();
            Mapper = _serviceProvider.GetService<IMapper>();
        }

        protected TService Resolve<TService>() => _serviceProvider.GetService<TService>();
    }
}
