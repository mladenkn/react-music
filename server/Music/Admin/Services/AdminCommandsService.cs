using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Music.Admin.Models;
using Music.App;

namespace Music.Admin.Services
{
    public class AdminCommandsService : ServiceResolverAware
    {
        public AdminCommandsService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<AdminYamlCommand>> Get()
        {
            var commands = new[]
            {
                new AdminYamlCommand
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
"
                },
                new AdminYamlCommand
                {
                    Name = "Query 2",
                    Yaml = @"# A list of tasty fruits
- Apple
- Orange
- Strawberry
- Mango
"
                },
                new AdminYamlCommand
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
"
                },
            };

            return commands;
        }
    }
}
