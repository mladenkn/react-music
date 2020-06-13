using System.Collections.Generic;

namespace Music.Models
{
    public class SaveTrackModel
    {
        public string YouTubeVideoId { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }

        public int Year { get; set; }
    }
}
