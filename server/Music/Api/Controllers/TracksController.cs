using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kernel;
using Microsoft.AspNetCore.Mvc;
using Music.Domain;
using Music.Domain.QueryTracksViaYoutube;
using Music.Domain.Shared;
using Utilities;

namespace Music.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracksController : ServiceResolverAware
    {
        public TracksController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public Task<ArrayWithTotalCount<TrackModel>> Get([FromQuery]QueryTracksRequest req) => Resolve<QueryTracksExecutor>().Execute(req);

        [HttpGet("yt")]
        public Task<IEnumerable<TrackModel>> QueryTracksViaYoutube([FromQuery]string searchQuery) =>
            Resolve<QueryTracksViaYoutubeExecutor>().Execute(searchQuery);

        [HttpPost]
        public Task Save(TrackUserPropsUpdateModel trackProps) => Resolve<SaveTrackYoutubeExecutor>().Execute(trackProps);
    }
}
