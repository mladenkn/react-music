using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Music;
using Xunit;

namespace Executables.FeatureTests
{
    public class QueryTracksViaYoutube
    {
        [Fact]
        public async Task Run()
        {
            var builder = new WebHostBuilder().UseStartup<Startup>();
            var server = new TestServer(builder);
            var client = server.CreateClient();
            var r = await client.GetAsync("api/tracks/yt?searchQuery=mia");
        }
    }
}
