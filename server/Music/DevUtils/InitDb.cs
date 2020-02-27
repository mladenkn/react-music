using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music.App;
using Music.App.DbModels;
using Music.App.Models;

namespace Music.DevUtils
{
    public class InitDb : ServiceResolverAware
    {
        public InitDb(IServiceProvider serviceProvider) : base(serviceProvider)
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
