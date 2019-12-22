using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kernel
{
    public abstract class RequestHandlerModule
    {
        private readonly IServiceProvider _serviceProvider;

        protected DbContext Db { get; }

        protected IMapper Mapper { get; }

        protected RequestHandlerModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Db = _serviceProvider.GetService<DbContext>();
            Mapper = _serviceProvider.GetService<IMapper>();
        }

        protected TService GetService<TService>() => _serviceProvider.GetService<TService>();

        protected void HandlerFor<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handle)
        {

        }
    }
}
