using System.Collections.Generic;
using Kernel;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;

namespace Music.Admin.Models
{
    public class AdminCommandDbModel : IDatabaseEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public string Name { get; set; }

        public string Yaml { get; set; }

        public static void Configure(ModelBuilder modelBuilder) =>
            modelBuilder.Entity<AdminCommandDbModel>(b =>
            {
                b.Property(m => m.Name).IsRequired();
                b.HasIndex(m => m.Name).IsUnique();
                b.ToTable("AdminCommands");
            });
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
