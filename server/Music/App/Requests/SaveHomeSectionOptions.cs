using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using Music.App.Models;
using Newtonsoft.Json;

namespace Music.App.Requests
{
    public class SaveHomeSectionOptions : ServiceResolverAware
    {
        public SaveHomeSectionOptions(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Save(HomeSectionPersistableStateModel opt)
        {
            var userId = Resolve<ICurrentUserContext>().Id;
            var user = await Query<User>().FirstOrDefaultAsync(u => u.Id == userId);
            user.HomeSectionStateJson = JsonConvert.SerializeObject(opt);
            await Persist(ops => ops.UpdateUsers(new []{ user }));
        }
    }
}
