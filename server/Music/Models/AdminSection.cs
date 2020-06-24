using System.Collections.Generic;

namespace Music.Models
{

    public class CsCommandForAdminSection
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }

    public class AdminSectionParams
    {
        public IEnumerable<CsCommandForAdminSection> Commands { get; set; }

        public int? CurrentCommandId { get; set; }
    }

    public class AdminSectionState
    {
        public int CurrentCommandId { get; set; }
    }
}
