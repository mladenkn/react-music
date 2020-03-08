using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Music
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var serviceScope = host.Services.CreateScope();
            Initialize(serviceScope.ServiceProvider).Wait();
            host.Run();
        }

        private static async Task Initialize(IServiceProvider sp)
        {
            //await new ResetDb(sp).Execute();
            //await new SaveAdminData(sp).Execute();
            //await new SaveTracks(sp).Execute();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
