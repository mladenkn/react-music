using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Music.DataAccess;
using Music.DataAccess.Models;

namespace Music
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var serviceScope = host.Services.CreateScope();
            var db = serviceScope.ServiceProvider.GetRequiredService<MusicDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Add(new User
            {
                Email = "mladen.knezovic.1993@gmail.com",
            });
            db.SaveChanges();

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
