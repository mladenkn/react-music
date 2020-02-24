using System;
using System.Threading.Tasks;
using Music.DataAccess.Models;

namespace Music.Domain
{
    public class HomeSectionOptionsRequests : ServiceResolverAware
    {
        public HomeSectionOptionsRequests(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<HomeSectionOptions> Get()
        {
            throw new NotImplementedException();
        }

        public async Task Save(HomeSectionOptions opt)
        {

        }
    }
}
