using System.Collections.Generic;
using Kernel;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;

namespace Music.Admin.Models
{
    public class UserAdminData : IDatabaseEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public IReadOnlyCollection<AdminCommand> Commands { get; set; }

        public string CurrentCommandName { get; set; }

        public static void Configure(ModelBuilder modelBuilder) =>
            modelBuilder.Entity<UserAdminData>(b =>
            {
                b.Property(e => e.CurrentCommandName).IsRequired();
            });
    }
}
