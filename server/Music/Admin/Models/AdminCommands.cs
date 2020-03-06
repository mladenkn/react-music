using System.Collections.Generic;

namespace Music.Admin.Models
{
    public class AdminYamlCommand
    {
        public string Name { get; set; }

        public string Yaml { get; set; }
    }

    public class AdminSectionParams
    {
        public IEnumerable<AdminYamlCommand> Commands { get; set; }

        public string CurrentCommandName { get; set; }

        public string CurrentCommandResponse { get; set; }
    }
}
