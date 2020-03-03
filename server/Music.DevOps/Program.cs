using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Music.App;
using Music.DevOps.Tasks;

namespace Music.DevOps
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            DoTasks(host).Wait();
        }

        public static async Task DoTasks(IHost host)
        {
            using var serviceScope = host.Services.CreateScope();
            var sp = serviceScope.ServiceProvider;

            await new ResetDb(sp).Execute();
            await new SaveTracks(sp).Execute();
            //await new PersistAllChannelsVideosToFile(sp).Execute();
            await new PersistChannelsFromFileToDb(sp).Execute();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
