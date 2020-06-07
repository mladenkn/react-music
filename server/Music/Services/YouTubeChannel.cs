using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3.Data;
using Microsoft.EntityFrameworkCore;
using Music.DbModels;
using Music.Models;
using Utilities;

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

        public async Task EnsureAreSaved(IEnumerable<Channel> channels)
        {
            var dbModels = channels.Select(YouTubeChannel.FromYouTubeApiChannel).ToArray();
            var unknownChannels = await FilterToUnknown(dbModels);
            await Persist(ops => unknownChannels.ForEach(ops.Add));
        }

        private async Task<IEnumerable<YouTubeChannel>> FilterToUnknown(IReadOnlyCollection<YouTubeChannel> channels)
        {
            var channelsIds = channels.Select(c => c.Id);
            var foundIds = await Query<YouTubeChannel>()
                .Select(v => v.Id)
                .Where(vId => channelsIds.Contains(vId))
                .ToArrayAsync();
            var notFoundIds = channels.Where(c => !foundIds.Contains(c.Id));
            return notFoundIds;
        }
    }
}
