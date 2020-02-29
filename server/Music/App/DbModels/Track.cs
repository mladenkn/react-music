using System.Collections.Generic;
using Kernel;

namespace Music.App.DbModels
{
    public class Track : IDatabaseEntity
    {
        public long Id { get; set; }

        public IReadOnlyCollection<TrackUserProps> TrackUserProps { get; set; }

        public IReadOnlyCollection<YoutubeVideo> YoutubeVideos { get; set; }
    }
}
