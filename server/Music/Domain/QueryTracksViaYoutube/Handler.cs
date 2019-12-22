using System;
using System.Collections.Generic;
using System.Linq;
using Kernel;
using Music.Domain.Shared;

namespace Music.Domain.QueryTracksViaYoutube
{
    public class QueryTracksViaYoutubeRequest
    {
        public string SearchQuery { get; set; }
    }

    public class QueryTracksViaYoutubeHandler : RequestHandlerModule
    {
        public QueryTracksViaYoutubeHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            HandlerFor<QueryTracksViaYoutubeRequest, IEnumerable<Track>>( async request =>
            {
                var service = GetService<QueryTracksViaYoutubeService>();

                var wantedTracksIds = (await service.SearchVideoIdsOnYt(request.SearchQuery)).ToArray();
                
                var notFoundVideosIds = await service.FilterToUnknownVideosIds(wantedTracksIds);
                var videosFromYt = await service.GetVideosFromYoutube(notFoundVideosIds.ToArray());
                await service.SaveVideos(videosFromYt);

                var tracks = await service.GetTracks(wantedTracksIds);

                return tracks;
            });
        }
    }
}
