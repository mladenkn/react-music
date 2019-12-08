using System.Collections.Generic;

namespace Music.Models
{
    public class Track
    {
        public string YtId { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public int? Year { get; set; }

        public IEnumerable<string> Genres { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public TrackChannel Channel { get; set; }
    }

    public class TrackChannel
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}
