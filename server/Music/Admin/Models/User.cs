using System.Collections.Generic;
using Kernel;
using Music.App.DbModels;

namespace Music.Admin.Models
{
    public class UserAdminData : IDatabaseEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public IReadOnlyCollection<AdminCommandDbModel> Commands { get; set; }

        public string CurrentCommandName { get; set; }
    }
}
