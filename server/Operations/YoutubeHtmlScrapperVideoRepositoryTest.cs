using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Executables
{
    public class YoutubeHtmlScrapperVideoRepositoryTest
    {
        [Fact]
        public async Task Search()
        {
            var services = new Services();
            var urls = await services.YoutubeHtmlScrapperVideoRepository.SearchIds("Aurora Borealis");
        }
    }
}
