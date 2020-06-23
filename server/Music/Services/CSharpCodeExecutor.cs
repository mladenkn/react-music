using System;
using Music.DbModels;
using Z.Expressions;

namespace Music.Services
{
    public class CSharpCodeExecutor : ServiceResolverAware
    {
        public CSharpCodeExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void Exectue(string code)
         {
         }
    }
}
