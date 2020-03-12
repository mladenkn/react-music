using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kernel
{
    public static class DependencyInversionUtils
    {
        public static void AddDelegateTransient<TDelegate, TDelegateContainer>(
            this IServiceCollection serviceCollection, Func<TDelegateContainer, TDelegate> chooseDelegate
            ) where TDelegate : class
        {
            serviceCollection.AddTransient<TDelegate>(serviceProvider =>
            {
                var delegateContainer = serviceProvider.GetService<TDelegateContainer>();
                var delegate_ = chooseDelegate(delegateContainer);
                return delegate_;
            });
        }
        
        public static void AddServiceResolverAwares(this IServiceCollection services, Assembly assembly, Func<Type, bool> isServicesResolverAware)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (isServicesResolverAware(type))
                {
                    var configureMethod = type.GetMethods().FirstOrDefault(m =>
                    {
                        var parameters = m.GetParameters();
                        return m.Name == "Configure" &&
                               m.IsStatic &&
                               parameters.Length == 1 &&
                               parameters.Single().ParameterType.IsAssignableFrom(typeof(IServiceCollection));
                    });
                    if (configureMethod != null)
                        configureMethod.Invoke(null, new object[] { services });
                    else
                        services.AddTransient(type);
                }
            }
        }
    }
}
