using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<ArrayWithTotalCount<TrackModel>> Get([FromQuery]QueryTracksRequest req)
        {
            var r = await Resolve<QueryTracksExecutor>().Execute(req);
            return r;
        }

        [HttpGet("yt")]
        public async Task<IEnumerable<TrackModel>> QueryTracksViaYoutube([FromQuery]string searchQuery)
        {
            var r = await Resolve<QueryTracksViaYoutubeExecutor>().Execute(searchQuery);
            return r;
        }

        [HttpPost]
        public Task<ArrayWithTotalCount<TrackModel>> Save([FromBody]SaveTrackRequest trackProps) => 
            Resolve<SaveTrackExecutor>().Execute(trackProps);
    }
}
