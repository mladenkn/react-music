using System;
using System.Collections.Generic;
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
        
        public static void AddServiceResolverAwares(
            this IServiceCollection services, 
            IEnumerable<Assembly> assemblies, 
            Func<Type, bool> isServicesResolverAware
        )
        {
            var types = assemblies.SelectMany(a => a.GetTypes());
            foreach (var type in types)
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
