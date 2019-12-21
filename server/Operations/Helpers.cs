using System.IO;
using System.Threading.Tasks;

namespace Executables
{
    public static class Helpers
    {
        public static async Task WriteToFile(string fileName, string fileContent)
        {
            var filePath = @"C:\Users\a\Documents\projekti\music\server\Operations\Files\" + fileName;
            await File.WriteAllTextAsync(filePath, fileContent);
        }
    }
}
