using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kernel
{
    public static class ServiceCollectionUtils
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
    }
}
