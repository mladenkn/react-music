using System.Collections.Generic;
using Kernel;

namespace Music.DbModels
{
    public class Track : IDatabaseEntity
    {
        public long Id { get; set; }

        public IReadOnlyCollection<TrackUserProps> TrackUserProps { get; set; }

        public IReadOnlyCollection<YoutubeVideo> YoutubeVideos { get; set; }

        // remove
        public bool IsIgnored { get; set; }
    }
}
