using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.Admin.Models;
using Music.App;
using Newtonsoft.Json;

namespace Music.Admin.Services
{
    public class AdminService : ServiceResolverAware
    {
        public AdminService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<AdminSectionParams> GetAdminSectionParams()
        {
            var userId = Resolve<ICurrentUserContext>().Id;

            var commands = await Query<AdminCommand>()
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.AddedAt)
                .Select(c => new AdminCommandForAdminSection
                {
                    Id = c.Id,
                    Name = c.Name,
                    Yaml = c.Yaml
                })
                .ToArrayAsync();

            var sectionStateJson = await Query<AdminUser>()
                .Where(u => u.Id == userId)
                .Select(u => u.AdminSectionStateJson)
                .FirstOrDefaultAsync();

            var currentCommandId = string.IsNullOrEmpty(sectionStateJson)
                ? (int?) null
                : JsonConvert.DeserializeObject<AdminSectionState>(sectionStateJson).CurrentCommandId;

            var r = new AdminSectionParams
            {
                Commands = commands,
                CurrentCommandId = currentCommandId
            };

            return r;
        }

        public async Task SaveSectionState(AdminSectionState state)
        {
            var userId = Resolve<ICurrentUserContext>().Id;
            var user = await Query<AdminUser>().FirstOrDefaultAsync(u => u.Id == userId);
            user.AdminSectionStateJson = JsonConvert.SerializeObject(state);
            Db.Update(user);
            await Db.SaveChangesAsync();
        }

        public async Task<AdminCommandForAdminSection> Add(AdminCommandForAdminSection cmd)
        {
            var userId = Resolve<ICurrentUserContext>().Id;
            var cmdDbEntity = new AdminCommand
            {
                UserId = userId,
                Name = cmd.Name,
                Yaml = cmd.Yaml,
                AddedAt = DateTime.Now
            };
            Db.Add(cmdDbEntity);
            await Db.SaveChangesAsync();
            return new AdminCommandForAdminSection
            {
                Id = cmdDbEntity.Id,
                Name = cmdDbEntity.Name,
                Yaml = cmdDbEntity.Yaml
            };
        }

        public async Task Update(AdminCommandForAdminSection cmd)
        {
            var userId = Resolve<ICurrentUserContext>().Id;
            
            var cmdFromDb = await Query<AdminCommand>()
                .FirstOrDefaultAsync(c => c.Id == cmd.Id);

            if (cmdFromDb == null)
                throw new ApplicationException("Command not found.");
            if(cmdFromDb.UserId != userId)
                throw new ApplicationException("Trying to update other user's command.");

            cmdFromDb.Name = cmd.Name;
            cmdFromDb.Yaml = cmd.Yaml;

            Db.Update(cmdFromDb);
            await Db.SaveChangesAsync();
        }
    }
}
