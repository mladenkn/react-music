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
            object result;

            try
            {
                var @delegate = c.Compile<Func<MusicDbContext, PersistantVariablesService, object>>(code, "db", "variables");
                result = @delegate(Db, variables);
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
