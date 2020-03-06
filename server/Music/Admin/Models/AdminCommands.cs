using System.Collections.Generic;
using Kernel;

namespace Music.Admin.Models
{
    public class AdminCommandDbModel : IDatabaseEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Yaml { get; set; }
    }

    public class AdminCommand
    {
        public string Name { get; set; }

        public string Yaml { get; set; }
    }

    public class AdminSectionParams
    {
        public IEnumerable<AdminCommand> Commands { get; set; }

        public string CurrentCommandName { get; set; }
    }
}
