using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kernel
{
    public interface IRequestHandler
    {
        Task<object> Handle(object request);
    }

    public abstract class RequestHandlerBase : IRequestHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private Func<object, Task<object>> _handler;

        protected DbContext Db { get; }

        protected IMapper Mapper { get; }

        protected RequestHandlerBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Db = _serviceProvider.GetService<DbContext>();
            Mapper = _serviceProvider.GetService<IMapper>();
        }

        protected TService GetService<TService>() => _serviceProvider.GetService<TService>();

        protected void RegisterHandler<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
        {

        }

        public Task<object> Handle(object request) => _handler(request);
    }
}
