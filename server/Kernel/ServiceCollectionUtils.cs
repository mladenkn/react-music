using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
