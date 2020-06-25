using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Z.Expressions;

namespace Music.Services
{
    public class CSharpCodeExecutor : ServiceResolverAware
    {
        public CSharpCodeExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void Register(EvalContext c)
        {
            c.RegisterAssembly(typeof(Startup).Assembly);
            c.RegisterAssembly(typeof(Utilities.ReflectionUtils).Assembly);
            c.RegisterExtensionMethod(typeof(EntityFrameworkQueryableExtensions));
            c.RegisterType(typeof(JsonConvert));
        }

        public async Task<object> Execute(string code)
        {
            var c = Resolve<EvalContext>();

            var store = Resolve<PersistantKeyValueStore>();
            var ytRemoteService = Resolve<YouTubeRemoteService>();
            var tracksService = Resolve<TracksService>();
            var ytVideosService = Resolve<YouTubeVideosService>();
            object result;

            try
            {
                var @delegate = c.Compile<Func<MusicDbContext, PersistantKeyValueStore, YouTubeRemoteService, TracksService, YouTubeVideosService, object>>(
                    code, "db", "PersistantKeyValueStore", "YouTubeRemoteService", "TracksService", "YouTubeVideosService"
                );
                result = @delegate(Db, store, ytRemoteService, tracksService, ytVideosService);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }

            switch (result)
            {
                case Task taskResult:
                {
                    await taskResult;
                    return result.GetType().IsGenericType ?
                        result.GetType().GetProperty("Result")!.GetValue(result) :
                        null;
                }
                default:
                    return result;
            }
        }
    }
}
