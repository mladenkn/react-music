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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Music.DbModels;
using Music.Services;
using Newtonsoft.Json;
using Z.Expressions;
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
        
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(o =>
                {
                    o.Filters.Add(new ExceptionToHttpResponseMapper());
                })
                .AddNewtonsoftJson(o =>
                {
                    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddDbContext<MusicDbContext>();

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
            services.AddServiceResolverAwares(
                new []{ typeof(Startup).Assembly }, 
                type => type.IsSubclassOf(typeof(ServiceResolverAware))
            );

            services.AddTransient<DataPersistor>();
            services.AddScoped<DbContext, MusicDbContext>();

            services.AddElmah();

            services.AddScoped(sp =>
            {
                var c = new EvalContext();
                sp.GetRequiredService<CSharpCodeExecutor>().Register(c);
                return c;
            });

            _reconfigureServices?.Invoke(services);
        }

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
