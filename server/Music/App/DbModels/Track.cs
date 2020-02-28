using System.Collections.Generic;
using Kernel;

namespace Music.App.DbModels
{
    [DatabaseEntity]
    public class Track
    {
        public long Id { get; set; }

        public IReadOnlyCollection<TrackUserProps> TrackUserProps { get; set; }

        public IReadOnlyCollection<YoutubeVideo> YoutubeVideos { get; set; }
    }
}
