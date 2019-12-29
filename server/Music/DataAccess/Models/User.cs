using System.ComponentModel.DataAnnotations;

namespace Music.DataAccess.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
