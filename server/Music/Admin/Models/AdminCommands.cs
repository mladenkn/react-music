using System.Collections.Generic;
using Kernel;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;

namespace Music.Admin.Models
{
    public class AdminCommand : IDatabaseEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public string Name { get; set; }

        public string Yaml { get; set; }

        public static void Configure(ModelBuilder modelBuilder) =>
            modelBuilder.Entity<AdminCommand>(b =>
            {
                b.Property(m => m.Name).IsRequired();
                b.HasIndex(m => m.Name).IsUnique();
            });
    }

    public class AdminCommandForAdminSection
    {
        public string Name { get; set; }

        public string Yaml { get; set; }
    }

    public class AdminSectionParams
    {
        public IEnumerable<AdminCommandForAdminSection> Commands { get; set; }

        public string CurrentCommandName { get; set; }
    }
}
