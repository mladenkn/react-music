using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kernel;
using Music.DataAccess;

namespace Music.Domain
{
    public class TrackUpdateModel
    {
        public long TrackYtId { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }
    }

    public class SaveTrackYoutubeExecutor : ServiceResolverAware<MusicDbContext>
    {
        public SaveTrackYoutubeExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Execute(TrackUpdateModel trackProps)
        {

        }
    }
}
