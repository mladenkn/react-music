using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.DbModels;
using Music.Models;

namespace Music.Services
{
    public class YouTubeChannelService : ServiceResolverAware
    {
        public YouTubeChannelService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<IEnumerable<object>> Get()
        {
            var r = await Query<YouTubeChannel>()
                .Select(YouTubeChannelForAdmin.Map)
                .ToListAsync();
            return r;
        }
    }
}
