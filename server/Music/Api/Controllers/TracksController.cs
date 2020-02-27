using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Music.Domain;
using Music.Domain.Shared;
using Music.Domain.Shared.Models;
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
        public Task<ArrayWithTotalCount<TrackModel>> Get([FromQuery]TracksQueryModel req) => 
            Resolve<QueryTracksExecutor>().Execute(req);

        [HttpGet("yt")]
        public Task<IEnumerable<TrackModel>> QueryTracksViaYoutube([FromQuery]string searchQuery) =>
            Resolve<QueryTracksViaYoutubeExecutor>().Execute(searchQuery);

        [HttpPost]
        public Task<ArrayWithTotalCount<TrackModel>> Save([FromBody]SaveTrackRequest trackProps) => 
            Resolve<SaveTrackExecutor>().Execute(trackProps);
    }
}
