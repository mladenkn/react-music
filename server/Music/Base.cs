using System;
using System.Threading.Tasks;
using Kernel;
using Microsoft.Extensions.DependencyInjection;

namespace Music
{
    public class ServiceResolverAware : ServiceResolverAware<MusicDbContext>
    {
        public ServiceResolverAware(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected Task Persist(Action<DataPersistorOperations> specifyOperations)
        {
            var persistor = Resolve<DataPersistor>();
            return persistor.Persist(specifyOperations);
        }
    }

    public class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public ControllerBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Resolve<T>() => _serviceProvider.GetRequiredService<T>();
    }
}
