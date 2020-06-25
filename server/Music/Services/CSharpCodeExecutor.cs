using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
            c.RegisterExtensionMethod(typeof(EntityFrameworkQueryableExtensions));
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
                    code, "db", "store", "ytRemote", "tracks", "ytVideos"
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
