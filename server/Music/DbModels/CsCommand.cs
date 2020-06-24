using System;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Music.DbModels
{
    public class CsCommand : IDatabaseEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public DateTime AddedAt { get; set; }

        public static void Configure(ModelBuilder modelBuilder) =>
            modelBuilder.Entity<CsCommand>(b =>
            {
                b.Property(m => m.Name).IsRequired();
                b.HasIndex(m => m.Name).IsUnique();
            });
    }
}
