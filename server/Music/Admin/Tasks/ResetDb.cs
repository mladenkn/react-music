using System;
using System.Threading.Tasks;
using Music.App;
using Music.App.DbModels;

namespace Music.Admin.Tasks
{
    public class ResetDb : ServiceResolverAware
    {
        public ResetDb(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Execute()
        {
            var db = Resolve<MusicDbContext>();

            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            await Persist(ops =>
            {
                var user = new User
                {
                    Email = "mladen.knezovic.1993@gmail.com",
                };
                ops.InsertUser(new []{user});
            });
        }
    }
}
