using System.Threading.Tasks;
using Kernel;
using McMaster.Extensions.CommandLineUtils;
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
            var host = CreateHostBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddServiceResolverAwares(typeof(Program).Assembly, type => type.IsSubclassOf(typeof(ServiceResolverAware)));
                })
                .Build();

            var app = new CommandLineApplication<Program>(throwOnUnexpectedArg: false);

            using var serviceScope = host.Services.CreateScope();
            var sp = serviceScope.ServiceProvider;

            PersistAllChannelsVideosToFile.ConfigureCommand(app, sp);

            app.Execute(args);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
