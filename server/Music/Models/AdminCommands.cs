using System.Collections.Generic;

namespace Music.Models
{

    public class AdminCommandForAdminSection
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Yaml { get; set; }
    }

    public class AdminSectionParams
    {
        public IEnumerable<AdminCommandForAdminSection> Commands { get; set; }

        public int? CurrentCommandId { get; set; }
    }
}
