using System;
using System.Threading.Tasks;
using Music.App;
using Music.App.DbModels;

namespace Music.DevUtils
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

            db.Add(new User
            {
                Email = "mladen.knezovic.1993@gmail.com",
            });
            
            await db.SaveChangesAsync();
        }
    }
}
