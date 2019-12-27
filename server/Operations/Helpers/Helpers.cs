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

        public static MusicDbContext UseDbContext()
        {
            var dbOptionsBuilder = new DbContextOptionsBuilder<MusicDbContext>();
            dbOptionsBuilder.UseSqlServer(Config.TestDatabaseConnectionString);
            return new MusicDbContext(dbOptionsBuilder.Options);
        }
    }
}
