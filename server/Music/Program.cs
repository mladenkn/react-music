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
            //var dbIniter = sp.GetRequiredService<DatabaseInitService>();
            //await dbIniter.ResetDb();
            //await dbIniter.SaveTracks();
            //await dbIniter.SaveAdminSectionData();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
