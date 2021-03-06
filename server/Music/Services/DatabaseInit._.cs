﻿using System;
using System.Threading.Tasks;
using Music.DbModels;

namespace Music.Services
{
    public partial class DatabaseInitService : ServiceResolverAware
    {
        public DatabaseInitService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task ResetDb()
        {
            var db = Resolve<MusicDbContext>();

            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            var user = new AdminUser
            {
                Email = "mladen.knezovic.1993@gmail.com",
            };
            await Persist(ops =>
            {
                ops.Add(user);
            });
        }
    }
}
