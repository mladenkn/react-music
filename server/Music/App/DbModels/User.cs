using System.ComponentModel.DataAnnotations;
using Kernel;

namespace Music.App.DbModels
{
    public class User : IDatabaseEntity
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        public string HomeSectionStateJson { get; set; }
    }
}
