using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Music.Services
{
    public class PersistantVariablesService : ServiceResolverAware
    {
        private readonly string _folderPath;

        public PersistantVariablesService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            var projFolder = Resolve<IWebHostEnvironment>().ContentRootPath;
            _folderPath = Path.Combine(projFolder, "..", "variable-files");
        }

        public async Task<T> Get<T>(string key)
        {
            var filePath = Path.Combine(_folderPath, key);
            var content = await File.ReadAllTextAsync(filePath);
            var deserialized = JsonConvert.DeserializeObject<T>(content);
            return deserialized;
        }

        public IEnumerable<string> GetAllKeys() => Directory.GetFiles(_folderPath).Select(Path.GetFileName);

        public async Task Set(string key, object value)
        {
            if (!Directory.Exists(_folderPath))
                Directory.CreateDirectory(_folderPath);
            var filePath = Path.Combine(_folderPath, key, ".json");
            
            if (File.Exists(filePath))
                File.Delete(filePath);

            var valueSerialized = JsonConvert.SerializeObject(value);
            await File.WriteAllTextAsync(filePath, valueSerialized);
        }
    }
}
