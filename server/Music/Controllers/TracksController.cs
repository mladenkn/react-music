using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Music.Models;
using Music.Services;
using Utilities;

namespace Music.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracksController : ServiceResolverAware
    {
        public TracksController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public Task<ArrayWithTotalCount<TrackForHomeSection>> Get([FromQuery]MusicDbTrackQueryParamsModel req) => 
            Resolve<TracksService>().QueryMusicDb(req);

        [HttpGet("yt")]
        public Task<IEnumerable<TrackForHomeSection>> QueryTracksViaYoutube([FromQuery]string searchQuery) =>
            Resolve<TracksService>().QueryViaYouTube(searchQuery);

        [HttpPost]
        public Task<ArrayWithTotalCount<TrackForHomeSection>> Save([FromBody]SaveTrackUserPropsModel trackProps) => 
            Resolve<TracksService>().SaveUserProps(trackProps);

        // PUT: api/Example/5
        [HttpPut("declareANonTrack/{id}")]
        public void Put(string id)
        {
        }
    }
}
