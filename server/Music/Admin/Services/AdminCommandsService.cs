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
                .SelectMany(d => d.Commands.Select(c => new AdminCommand { Name = c.Name, Yaml = c.Yaml }))
                .ToArrayAsync();

            var r = new AdminSectionParams
            {
                Commands = commands,
                CurrentCommandName = commands.First().Name,
                CurrentCommandResponse = @"doe: 'a deer, a female deer'
ray: 'a drop of golden sun'
pi: 3.14159
xmas: true
french-hens: 3
calling-birds: 
  - huey
  - dewey
  - louie
  - fred
xmas-fifth-day: 
  calling-birds: four
  french-hens: 3
  golden-rings: 5
  partridges: 
    count: 1
    location: 'a pear tree'
  turtle-doves: two
",
            };

            return r;
        }
    }
}
