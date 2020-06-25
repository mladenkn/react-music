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

            var variables = Resolve<PersistantVariablesService>();
            var @delegate = c.Compile<Func<MusicDbContext, PersistantVariablesService, object>>(code, "db", "variables");
            var r = @delegate(Db, variables);

            switch (r)
            {
                case Task taskResult:
                {
                    await taskResult;
                    return r.GetType().IsGenericType ?
                        r.GetType().GetProperty("Result")!.GetValue(r) :
                        null;
                }
                default:
                    return r;
            }
        }
    }
}
