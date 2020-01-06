using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Music;
using Xunit;

namespace Executables.Tests
{
    public class ServiceResolverTest
    {
        [Fact]
        public void CanResolveEachServiceOnOneScope()
        {
            var builder = new WebHostBuilder().UseStartup<Startup>();
            using var server = new TestServer(builder);
            using var serviceScope = server.Services.CreateScope();

            var allServiceTypes = GetAllServiceTypes(builder);

            foreach (var serviceType in allServiceTypes) 
                serviceScope.ServiceProvider.GetService(serviceType);
        }

        [Fact]
        public void CanResolveEachServiceOnSeparateScope()
        {
            var builder = new WebHostBuilder().UseStartup<Startup>();
            using var server = new TestServer(builder);
            var allServiceTypes = GetAllServiceTypes(builder);
            foreach (var serviceType in allServiceTypes)
            {
                using var serviceScope = server.Services.CreateScope();
                serviceScope.ServiceProvider.GetService(serviceType);
            }
        }

        private IEnumerable<Type> GetAllServiceTypes(IWebHostBuilder webHostBuilder)
        {
            var r = new List<Type>();
            webHostBuilder.ConfigureServices(serviceCollection =>
            {
                foreach (var serviceDescriptor in serviceCollection)
                    r.Add(serviceDescriptor.ServiceType);
            });
            return r;
        }
    }
}
