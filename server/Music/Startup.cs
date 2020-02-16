using System.Linq;
using System.Net.Http;
using AngleSharp;
using AutoMapper;
using ElmahCore.Mvc;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Kernel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Music.Api;
using Music.DataAccess;
using Music.Domain;
using Music.Domain.PersistYoutubeVideos;
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

            services.AddDbContext<MusicDbContext>(MusicDbContext.Configure);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Music API", Version = "v1" });
            });
            services.AddAutoMapper(typeof(YoutubeVideoMapperProfile).Assembly);
            services.AddTransient<HttpClient>();
            services.AddTransient(sp => BrowsingContext.New(AngleSharp.Configuration.Default));

            services.AddTransient<YouTubeService>(_ => new YouTubeService(new BaseClientService.Initializer
                {
                    ApiKey = "AIzaSyA1xQd0rfJCzG1ghK7RoKRI7EfakGLfDZM"
                }
            ));
            services.AddTransient<ICurrentUserContext, CurrentUserContextMock>();
            AddServiceResolverAwares(services);

            services.AddElmah();

            _reconfigureServices?.Invoke(services);
        }

        public void AddServiceResolverAwares(IServiceCollection services)
        {
            var assembly = typeof(QueryTracksExecutor).Assembly;
            
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(ServiceResolverAware)))
                {
                    services.AddTransient(type);
                    var configureMethod = type.GetMethods().FirstOrDefault(m =>
                    {
                        var parameters = m.GetParameters();
                        return m.Name == "Configure" &&
                               m.IsStatic &&
                               parameters.Length == 1 && 
                               parameters.Single().ParameterType != typeof(IServiceCollection);
                    });
                    if (configureMethod != null)
                        configureMethod.Invoke(null, new object[]{ services });
                }
            }
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
