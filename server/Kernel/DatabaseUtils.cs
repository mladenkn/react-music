using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Kernel
{
    public interface IDatabaseEntity
    {
    }

    public static class DatabaseUtils
    {
        public static void ConfigureEntities(this ModelBuilder modelBuilder, Assembly assembly)
        {
           foreach (var type in assembly.GetTypes())
           {
               var isDbEntity = type.GetInterfaces().Count(t => t == typeof(IDatabaseEntity)) == 1;
               if (isDbEntity)
               {
                   modelBuilder.Entity(type);
                   var configureMethod = type.GetMethods().FirstOrDefault(m =>
                   {
                        var parameters = m.GetParameters();
                        return m.Name == "Configure" &&
                               m.IsStatic &&
                               parameters.Length == 1 &&
                               parameters.Single().ParameterType == typeof(ModelBuilder);
                   });
                   if (configureMethod != null)
                        configureMethod.Invoke(null, new object[] { modelBuilder });
               }
            }
        }
    }
}
