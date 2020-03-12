using System;
using System.Threading.Tasks;
using Music.Admin.Models;

namespace Music.Admin.Services
{
    public partial class DatabaseInit : ServiceResolverAware
    {
        public DatabaseInit(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task ResetDb()
        {
            var db = Resolve<MusicDbContext>();

            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            await Persist(ops =>
            {
                var user = new AdminUser
                {
                    Email = "mladen.knezovic.1993@gmail.com",
                };
                ops.InsertUser(new[] { user });
            });
        }
    }
}
