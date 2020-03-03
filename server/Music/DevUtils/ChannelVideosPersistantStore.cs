using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Music.App;
using Music.App.Models;
using Newtonsoft.Json;

namespace Music.DevUtils
{
    public class ChannelVideosPersistantStore : ServiceResolverAware
    {
        private readonly string _folder;
        public ChannelVideosPersistantStore(IServiceProvider sp) : base(sp)
        {
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            _folder = Path.Combine(env.ContentRootPath, "..", "..", "data-files", "videos-by-channels");
        }

        public async Task Store(YouTubeChannelWithVideos channel)
        {
            Directory.CreateDirectory(_folder);
            var filePath = Path.Combine(_folder, $"{channel.Title} - {channel.Id}.json");
            var channelJson = JsonConvert.SerializeObject(channel, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, channelJson);
        }

        private async Task<YouTubeChannelWithVideos> GetOne(string filePath)
        {
            var channelJson = await File.ReadAllTextAsync(filePath);
            var channel = JsonConvert.DeserializeObject<YouTubeChannelWithVideos>(channelJson);
            return channel;
        }

        public async Task<IReadOnlyList<YouTubeChannelWithVideos>> GetAll()
        {
            var files = Directory.GetFiles(_folder);
            var r = new List<YouTubeChannelWithVideos>();
            foreach (var file in files)
                r.Add(await GetOne(file));
            return r;
        }
    }
}
