using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Music.Domain.Models;
using Music.Domain.Services;

namespace Music.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracksController : ControllerBase
    {
        private readonly TrackService _trackService;

        public TracksController(TrackService trackService)
        {
            _trackService = trackService;
        }

        [HttpGet]
        public Task<GetTrackListResponse> Get([FromQuery] GetTracksArguments args) => _trackService.GetList(args);

        [HttpGet("yt")]
        public Task<SearchYoutubeResult> SearchYoutube([FromQuery] YoutubeTrackQuery args) => _trackService.SearchYoutube(args);

        [HttpPost]
        public Task<TrackPermissions> Save(Post req) => _trackService.Save(req.Tracks);
    }

    public class Post
    {
        public IEnumerable<TrackUserProps> Tracks { get; set; }
    }
}
