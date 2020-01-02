using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess;

namespace Executables.Helpers
{
    public static class Utils
    {
        public static async Task WriteToFile(string fileName, string fileContent)
        {
            var filePath = @"C:\Users\a\Documents\projekti\music\server\Operations\Files\" + fileName;
            await File.WriteAllTextAsync(filePath, fileContent);
        }

        public static MusicDbContext UseDatabase(string dbName)
        {
            var dbOptionsBuilder = new DbContextOptionsBuilder<MusicDbContext>();
            dbOptionsBuilder.UseSqlServer(Config.GetTestDatabaseConnectionString(dbName));
            return new MusicDbContext(dbOptionsBuilder.Options);
        }

        public static async Task UseDatabase(params Func<MusicDbContext, Task>[] actions)
        {
            var dbName = Guid.NewGuid().ToString();

            for (var i = 0; i < actions.Length; i++)
            {
                using var db = UseDatabase(dbName);
                
                if (i == 0)
                {
                    await db.Database.EnsureDeletedAsync();
                    await db.Database.EnsureCreatedAsync();
                }
                
                await actions[i](db);
                
                if (i == (actions.Length - 1))
                    await db.Database.EnsureDeletedAsync();
            }
        }
    }
}
