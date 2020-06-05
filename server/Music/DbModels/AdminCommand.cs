using System;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Music.DbModels
{
    public class AdminCommand : IDatabaseEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public string Name { get; set; }

        public string Yaml { get; set; }

        public DateTime AddedAt { get; set; }

        public static void Configure(ModelBuilder modelBuilder) =>
            modelBuilder.Entity<AdminCommand>(b =>
            {
                b.Property(m => m.Name).IsRequired();
                b.HasIndex(m => m.Name).IsUnique();
            });
    }
}
