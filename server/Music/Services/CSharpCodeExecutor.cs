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
            object result;

            try
            {
                var @delegate = c.Compile<Func<MusicDbContext, PersistantKeyValueStore, object>>(code, "db", "store");
                result = @delegate(Db, store);
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
