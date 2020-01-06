using System.Net.Http;
using AngleSharp;
using AutoMapper;
using ElmahCore.Mvc;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Kernel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Music.Api;
using Music.DataAccess;
using Music.Domain;
using Music.Domain.QueryTracksViaYoutube;
using Music.Domain.Shared;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Music
{
    public class Startup
    {
        public delegate void ReconfigureServices(IServiceCollection serviceCollection);

        private readonly ReconfigureServices _reconfigureServices;

        public Startup(IConfiguration configuration, ReconfigureServices reconfigureServices = null)
        {
            _reconfigureServices = reconfigureServices;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc(o => o.EnableEndpointRouting = true);

            services.AddControllers(o => o.Filters.Add(new ExceptionToHttpResponseMapper()));

            services.AddSingleton<IMongoDatabase>(_ =>
            {
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("music");
                return database;
            });

            services.AddDbContext<MusicDbContext>(o => o.UseSqlServer($"Data Source=DESKTOP-VSBO5TE\\SQLEXPRESS;Initial Catalog=MusicTest;Integrated Security=True"));
            services.AddTransient<DataPersistor>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Music API", Version = "v1" });
            });
            services.AddAutoMapper(typeof(YoutubeVideoMapperProfile).Assembly);
            services.AddTransient<HttpClient>();
            services.AddTransient<IBrowsingContext>(sp => BrowsingContext.New(AngleSharp.Configuration.Default));

            services.AddTransient<YouTubeService>(_ => new YouTubeService(new BaseClientService.Initializer
                {
                    ApiKey = "AIzaSyA1xQd0rfJCzG1ghK7RoKRI7EfakGLfDZM"
                }
            ));
            services.AddTransient<QueryTracksViaYoutubeServices>();
            services.AddDelegateTransient<SearchYoutubeVideosIds, QueryTracksViaYoutubeServices>(s => s.SearchYoutubeVideosIds);
            services.AddDelegateTransient<ListYoutubeVideos, QueryTracksViaYoutubeServices>(s => s.ListYoutubeVideos);

            services.AddTransient<QueryTracksExecutor>();
            services.AddTransient<QueryTracksViaYoutubeExecutor>();
            services.AddTransient<SaveTrackYoutubeExecutor>();

            services.AddTransient<ICurrentUserContext, CurrentUserContextMock>();

            services.AddElmah();

            _reconfigureServices?.Invoke(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Music API V1");
            });

            app.UseCors(o => o
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseElmah();
        }
    }
}
