using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Music;
using Music.DataAccess;
using Utilities;

namespace Executables.Helpers
{
    public class ServerTestOptions
    {
        public IEnumerable<Action<IServiceCollection>> ConfigureServices { get; } = new List<Action<IServiceCollection>>();
        public IEnumerable<Func<MusicDbContext, Task>> PrepareDatabase { get; } = new List<Func<MusicDbContext, Task>>();
        public Func<HttpClient, Task<HttpResponseMessage>> Act { get; set; }
        public IEnumerable<Func<HttpResponseMessage, MusicDbContext, Task>> Assert { get; } = new List<Func<HttpResponseMessage, MusicDbContext, Task>>();
    }

    public class ServerTestOptionsBuilder
    {
        private readonly ServerTestOptions _options = new ServerTestOptions();

        public ServerTestOptionsBuilder ConfigureServices(Action<IServiceCollection> configureServicesActual)
        {
            ((List<Action<IServiceCollection>>) _options.ConfigureServices).Add(configureServicesActual);
            return this;
        }

        public ServerTestOptionsBuilder PrepareDatabase(Func<MusicDbContext, Task> prepareDatabaseActual)
        {
            ((List<Func<MusicDbContext, Task>>)_options.PrepareDatabase).Add(prepareDatabaseActual);
            return this;
        }

        public ServerTestOptionsBuilder Act(Func<HttpClient, Task<HttpResponseMessage>> actActual)
        {
            _options.Act = actActual;
            return this;
        }

        public ServerTestOptionsBuilder Assert(Func<HttpResponseMessage, MusicDbContext, Task> assertActual)
        {
            ((List<Func<HttpResponseMessage, MusicDbContext, Task>>)_options.Assert).Add(assertActual);
            return this;
        }

        public ServerTestOptions Build() => _options;
    }

    public class ServerTest
    {
        public static async Task Run(Action<ServerTestOptionsBuilder> addOptions)
        {
            var optionsBuilder = new ServerTestOptionsBuilder();
            addOptions(optionsBuilder);
            var options = optionsBuilder.Build();
            await Run(options);
        }

        public static Task Run(ServerTestOptionsBuilder optionsBuilder) => Run(optionsBuilder.Build());

        public static async Task Run(ServerTestOptions options)
        {
            await using var dbClient = await TestDatabaseClient.Create();

            await dbClient.UseIt(async db =>
            {
                foreach (var action in options.PrepareDatabase)
                    await action(db);
            });

            var builder = new WebHostBuilder().UseStartup<Startup>();

            void ReconfigureServices(IServiceCollection services) 
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MusicDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
                services.AddDbContext<MusicDbContext>(o => o.UseSqlServer(Config.GetTestDatabaseConnectionString(dbClient.DatabaseName)));
                options.ConfigureServices.ForEach(action => action(services));
            }

            builder.ConfigureServices(services =>
            {
                services.AddTransient<Startup.ReconfigureServices>(sp => ReconfigureServices);
            });

            using var server = new TestServer(builder);
            using var client = server.CreateClient();
            
            var serverResponse = await options.Act(client);

            await dbClient.UseIt(async db =>
            {
                foreach (var action in options.Assert)
                    await action(serverResponse, db);
                await db.Database.EnsureDeletedAsync();
            });
        }
    }
}
