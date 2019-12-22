using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kernel
{
    public abstract class ServiceBase
    {
        private readonly IServiceProvider _serviceProvider;

        protected DbContext Db { get; }

        protected IMapper Mapper { get; }

        protected ServiceBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Db = _serviceProvider.GetService<DbContext>();
            Mapper = _serviceProvider.GetService<IMapper>();
        }

        protected TService Resolve<TService>() => _serviceProvider.GetService<TService>();
    }
}
