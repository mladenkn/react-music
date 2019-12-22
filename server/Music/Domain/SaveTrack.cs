using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Music.Domain.Shared;

namespace Music.Domain
{
    public class TrackUserProps
    {
        public long TrackYtId { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }
    }

    public class SaveTrackRequest : IRequest<IEnumerable<Track>>
    {
        public TrackUserProps Track { get; set; }
    }

    public class SaveTrackYoutubeHandler : IRequestHandler<QueryTracksRequest, IEnumerable<Track>>
    {
        public Task<IEnumerable<Track>> Handle(QueryTracksRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
