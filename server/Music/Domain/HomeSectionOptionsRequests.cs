using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.Domain.Shared;
using Music.Domain.Shared.Models;
using Newtonsoft.Json;

namespace Music.Domain
{
    public class HomeSectionOptionsRequests : ServiceResolverAware
    {
        public HomeSectionOptionsRequests(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Save(HomeSectionPersistableStateModel opt)
        {
            var userId = Resolve<ICurrentUserContext>().Id;
            var user = await Db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            user.HomeSectionStateJson = JsonConvert.SerializeObject(opt);
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }
    }
}
