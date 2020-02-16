using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Kernel
{
    public class DatabaseEntity : Attribute
    {
    }

    public static class DatabaseUtils
    {
        public static void ConfigureEntities(this ModelBuilder modelBuilder, Assembly assembly)
        {
           foreach (var type in assembly.GetTypes())
           {
                var entityAttribute = type.GetCustomAttributes(typeof(DatabaseEntity), true).FirstOrDefault();
                if (entityAttribute != null)
                {
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
