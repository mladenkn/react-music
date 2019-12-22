using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kernel;
using MediatR;
using Music.Domain.Shared;

namespace Music.Domain.QueryTracksViaYoutube
{
    public class QueryTracksViaYoutubeRequest : IRequest<IEnumerable<Track>>
    {
        public string SearchQuery { get; set; }
    }

    public class QueryTracksViaYoutubeHandler : RequestHandlerBase<QueryTracksViaYoutubeRequest, IEnumerable<Track>>
    {
        public QueryTracksViaYoutubeHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override async Task<IEnumerable<Track>> Handle(QueryTracksViaYoutubeRequest request, CancellationToken cancellationToken)
        {
            var service = GetService<QueryTracksViaYoutubeService>();

            var wantedTracksIds = await service.SearchVideoIdsOnYt(request.SearchQuery);
            var (videosFromDb, notFoundVideosIds) = await service.GetKnownVideos(wantedTracksIds);
            var videosFromYt = await service.GetVideosFromYoutube(notFoundVideosIds.ToArray());
            await service.SaveVideos(videosFromYt);
            var tracks = await service.VideosToTracks(videosFromDb.Concat(videosFromYt));
            return tracks;
        }
    }
}
