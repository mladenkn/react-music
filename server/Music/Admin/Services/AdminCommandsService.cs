using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.Admin.Models;
using Music.App;

namespace Music.Admin.Services
{
    public class AdminCommandsService : ServiceResolverAware
    {
        public AdminCommandsService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<AdminSectionParams> GetAdminSectionParams()
        {
            var userId = Resolve<ICurrentUserContext>().Id;

            var commands = await Query<UserAdminData>()
                .Where(d => d.UserId == userId)
                .SelectMany(d => d.Commands.Select(c => new AdminCommandForAdminSection { Name = c.Name, Yaml = c.Yaml }))
                .ToArrayAsync();

            var r = new AdminSectionParams
            {
                Commands = commands,
                CurrentCommandName = commands.First().Name
            };

            return r;
        }

        public async Task Add(AdminCommandForAdminSection cmd)
        {
            var userId = Resolve<ICurrentUserContext>().Id;
            var cmdDbEntity = new AdminCommand
            {
                UserId = userId,
                Name = cmd.Name,
                Yaml = cmd.Yaml,
            };
            Db.Add(cmdDbEntity);
            await Db.SaveChangesAsync();
        }

        public async Task Update(AdminCommandForAdminSection cmd)
        {
            var userId = Resolve<ICurrentUserContext>().Id;
            var cmdFromDb = await Query<AdminCommand>()
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if(cmdFromDb == null)
                throw new ApplicationException("Command not found.");
            cmdFromDb.Name = cmd.Name;
            cmdFromDb.Yaml = cmd.Yaml;

            Db.Update(cmdFromDb);
            await Db.SaveChangesAsync();
        }
    }
}
