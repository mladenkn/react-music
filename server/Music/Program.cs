using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Music.App;
using Music.DevUtils;

namespace Music
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            DoTasks(host).Wait();
            host.Run();
        }

        public static async Task DoTasks(IHost host)
        {
            using var serviceScope = host.Services.CreateScope();
            var sp = serviceScope.ServiceProvider;

            var env = sp.GetRequiredService<IWebHostEnvironment>();
            var videoFilesFolder = Path.Combine(env.ContentRootPath, "..", "data-files", "videos-by-channels");
            Directory.CreateDirectory(videoFilesFolder);

            //await new InitDb(sp).Execute();
            //await new SaveTracks(sp).Execute();
            await new PersistAllChannelsVideosToFile(sp).Execute(videoFilesFolder);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
