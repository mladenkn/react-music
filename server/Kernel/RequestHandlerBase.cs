using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Kernel
{
    public abstract class RequestHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IServiceProvider _serviceProvider;

        protected DbContext Db { get; }

        protected IMapper Mapper { get; }

        protected RequestHandlerBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Db = _serviceProvider.GetService<DbContext>();
            Mapper = _serviceProvider.GetService<IMapper>();
        }

        protected TService GetService<TService>() => _serviceProvider.GetService<TService>();

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
