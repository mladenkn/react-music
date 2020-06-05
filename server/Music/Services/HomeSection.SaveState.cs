using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.DbModels;
using Music.Models;
using Newtonsoft.Json;

namespace Music.Services
{
    public partial class HomeSectionService
    {
        public async Task SaveState(HomeSectionPersistableStateModel opt)
        {
            var userId = Resolve<ICurrentUserContext>().Id;
            var user = await Query<User>().FirstOrDefaultAsync(u => u.Id == userId);
            user.HomeSectionStateJson = JsonConvert.SerializeObject(opt);
            await Persist(ops => ops.Update(user));
        }
    }
}
