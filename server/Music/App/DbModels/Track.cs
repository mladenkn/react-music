using System.Collections.Generic;
using Kernel;
using Music.App.Models;

namespace Music.App.DbModels
{
    [DatabaseEntity]
    public class Track
    {
        public int Id { get; set; }

        public int TrackUserPropsId { get; set; }

        public TrackUserProps TrackUserProps { get; set; }

        public List<YoutubeVideo> YoutubeVideos { get; set; }
    }
}
