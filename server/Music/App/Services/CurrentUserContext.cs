using System;
using Microsoft.Extensions.DependencyInjection;

namespace Music.App.Services
{
    public interface ICurrentUserContext
    {
        int Id { get; }
    }

    public class CurrentUserContextMock : ServiceResolverAware, ICurrentUserContext
    {
        public CurrentUserContextMock(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public int Id => 1;

        public static void Configure(IServiceCollection services)
        {
            services.AddTransient<ICurrentUserContext, CurrentUserContextMock>();
        }
    }
}
