using System.Net.Http;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Music.Operations;
using Music.Repositories;
using Music.Services;

namespace Music
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IMongoDatabase>(_ =>
            {
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("music");
                return database;
            });

            services.AddTransient<MigrationToRelational>();

            services.AddTransient<YouTubeService>(_ => new YouTubeService(new BaseClientService.Initializer
                {
                    ApiKey = "AIzaSyA1xQd0rfJCzG1ghK7RoKRI7EfakGLfDZM"
                }
            ));

            services.AddTransient<HttpClient>();
            services.AddTransient<TrackService>();
            services.AddTransient<MongoTrackRepository>();
            services.AddTransient<TrackRepository>();
            services.AddTransient<YoutubeVideoMasterRepository>();
            services.AddTransient<YoutubeDataApiVideoRepository>();
            services.AddTransient<YoutubeVideoMongoRepository>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            ;
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
