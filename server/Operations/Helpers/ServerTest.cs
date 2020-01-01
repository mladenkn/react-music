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
        public IEnumerable<Action<MusicDbContext>> PrepareDatabase { get; } = new List<Action<MusicDbContext>>();
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

        public ServerTestOptionsBuilder PrepareDatabase(Action<MusicDbContext> prepareDatabaseActual)
        {
            ((List<Action<MusicDbContext>>)_options.PrepareDatabase).Add(prepareDatabaseActual);
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
            var builder = new WebHostBuilder().UseStartup<Startup>();

            var dbName = Guid.NewGuid().ToString();

            using (var db = Utils.UseDbContext(dbName))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                options.PrepareDatabase.ForEach(action => action(db));
            }

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MusicDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
                services.AddDbContext<MusicDbContext>(o => o.UseSqlServer(Config.GetTestDatabaseConnectionString(dbName)));
                options.ConfigureServices.ForEach(action => action(services));
            });

            var server = new TestServer(builder);
            var client = server.CreateClient();
            
            var serverResponse = await options.Act(client);

            using (var db = Utils.UseDbContext(dbName))
            {
                options.Assert.ForEach(action => action(serverResponse, db));
                db.Database.EnsureDeleted();
            }

            server.Dispose();
            client.Dispose();
        }
    }
}
