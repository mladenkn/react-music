using Music.App.DbModels;

namespace Music.Admin.Models
{
    public class AdminUser : User
    {
        public string AdminSectionStateJson { get; set; }
    }
}
