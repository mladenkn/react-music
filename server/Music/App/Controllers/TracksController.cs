using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Music.App.Models;
using Music.App.Services;
using Utilities;

namespace Music.App.Controllers
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
        public async Task<IEnumerable<TrackForHomeSection>> QueryTracksViaYoutube([FromQuery]string searchQuery)
        {
            var service = Resolve<TracksService>();
            var videoIds = await service.LearnYtVideos(searchQuery);
            return await service.GetTracksWithVideoIds(videoIds);
        }

        [HttpPost]
        public Task<ArrayWithTotalCount<TrackForHomeSection>> Save([FromBody]SaveTrackUserPropsModel trackProps) => 
            Resolve<TracksService>().SaveUserProps(trackProps);
    }
}
