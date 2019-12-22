using System.Collections.Generic;

namespace Music.Domain.Models
{
    public class Track
    {
        public string YtId { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }

        public TrackChannel Channel { get; set; }
    }

    public class TrackUserProps
    {
        public long TrackYtId { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }
    }

    public class TrackChannel
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class TrackPermissions
    {
        public bool CanEditTrackData { get; } = true;
        public bool CanFetchTrackRecommendations { get; } = true;
    }
}
