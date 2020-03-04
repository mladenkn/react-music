using Kernel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Music.App;

namespace Music.DevOps
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddServiceResolverAwares(typeof(Program).Assembly, type => type.IsSubclassOf(typeof(ServiceResolverAware)));
                })
                .Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
