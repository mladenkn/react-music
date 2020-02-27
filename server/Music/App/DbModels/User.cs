using System.ComponentModel.DataAnnotations;

namespace Music.App.DbModels
{
    public class User
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        public string HomeSectionStateJson { get; set; }
    }
}
