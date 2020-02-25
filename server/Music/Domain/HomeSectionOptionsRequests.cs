using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Models;
using Music.Domain.Shared;
using Newtonsoft.Json;

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
            var userId = Resolve<ICurrentUserContext>().Id;
            var user = await Db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            user.HomeSectionOptionsJson = JsonConvert.SerializeObject(opt);
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }
    }
}
