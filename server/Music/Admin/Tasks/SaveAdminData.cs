using System;
using System.Linq;
using System.Threading.Tasks;
using Music.Admin.Models;
using Utilities;

namespace Music.Admin.Tasks
{
    public class SaveAdminData : ServiceResolverAware
    {
        public SaveAdminData(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Execute()
        {
            var commands = new[]
            {
                new AdminCommand
                {
                    Name = "Query 1",
                    Yaml = @"name: Martin D'vloper
job: Developer
skill: Elite
employed: True
foods:
  - Apple
  - Orange
  - Strawberry
  - Mango
languages:
  perl: Elite
  python: Elite
  pascal: Lame
education: |
  4 GCSEs
  3 A-Levels
  BSc in the Internet of Things 
",
                    UserId = 1,
                },
                new AdminCommand
                {
                    Name = "Query 2",
                    Yaml = @"# A list of tasty fruits
- Apple
- Orange
- Strawberry
- Mango
",
                    UserId = 1,
                },
                new AdminCommand
                {
                    Name = "Query 3",
                    Yaml = @"# Employee records
-  martin:
    name: Martin D'vloper
    job: Developer
    skills:
      - python
      - perl
      - pascal
-  tabitha:
    name: Tabitha Bitumen
    job: Developer
    skills:
      - lisp
      - fortran
      - erlang
",
                    UserId = 1,
                },
            };
            
            var db = Resolve<MusicDbContext>();

            commands.ForEach(c => db.Add(c));
            await db.SaveChangesAsync();
        }
    }
}
