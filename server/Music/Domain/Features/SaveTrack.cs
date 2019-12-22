using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Music.Domain.Models;

namespace Music.Domain.Features
{
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
