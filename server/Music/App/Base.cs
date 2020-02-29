using System;
using System.Threading.Tasks;
using Kernel;

namespace Music.App
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
}
