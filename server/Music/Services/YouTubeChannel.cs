using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music.DbModels;
using Music.Models;

namespace Music.Services
{
    public class YouTubeChannelService : ServiceResolverAware
    {
        public YouTubeChannelService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
