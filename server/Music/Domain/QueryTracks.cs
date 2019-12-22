using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Music.Domain.Models;
using Music.Domain.Shared;
using Utilities;

namespace Music.Domain
{
    public class QueryTracksRequest : IRequest<IEnumerable<Track>>
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public IEnumerable<string> MustHaveEveryTag { get; set; }

        public IEnumerable<string> MustHaveAnyTag { get; set; }

        public Range<int> YearInRange { get; set; }
    }

    public class QueryTracksHandler : IRequestHandler<QueryTracksRequest, IEnumerable<Track>>
    {
        public Task<IEnumerable<Track>> Handle(QueryTracksRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
