﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Music;
using Music.DataAccess;

namespace Executables.Helpers
{
    public class ServerTestOptions
    {
        public Action<IServiceCollection> ConfigureServices { get; set; } = services => { };
        public Action<MusicDbContext> PrepareDatabase { get; set; } = db => { };
        public Func<HttpClient, Task<HttpResponseMessage>> Act { get; set; }
        public Func<HttpResponseMessage, MusicDbContext, Task> Assert { get; set; }
    }

    public class ServerTestOptionsBuilder
    {
        private readonly ServerTestOptions _options = new ServerTestOptions();

        public ServerTestOptionsBuilder ConfigureServices(Action<IServiceCollection> configureServicesActual)
        {
            _options.ConfigureServices = configureServicesActual;
            return this;
        }

        public ServerTestOptionsBuilder PrepareDatabase(Action<MusicDbContext> prepareDatabaseActual)
        {
            _options.PrepareDatabase = prepareDatabaseActual;
            return this;
        }

        public ServerTestOptionsBuilder Act(Func<HttpClient, Task<HttpResponseMessage>> actActual)
        {
            _options.Act = actActual;
            return this;
        }

        public ServerTestOptionsBuilder Assert(Func<HttpResponseMessage, MusicDbContext, Task> assertActual)
        {
            _options.Assert = assertActual;
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

        public static async Task Run(ServerTestOptions options)
        {
            var builder = new WebHostBuilder().UseStartup<Startup>();

            using (var db = Utils.UseDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                options.PrepareDatabase(db);
            }

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MusicDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
                services.AddDbContext<MusicDbContext>(o => o.UseSqlServer(Config.TestDatabaseConnectionString));
                options.ConfigureServices(services);
            });

            var server = new TestServer(builder);
            var client = server.CreateClient();
            
            var serverResponse = await options.Act(client);

            using (var db = Utils.UseDbContext())
            {
                await options.Assert(serverResponse, db);
                db.Database.EnsureDeleted();
            }

            server.Dispose();
            client.Dispose();
        }
    }
}
