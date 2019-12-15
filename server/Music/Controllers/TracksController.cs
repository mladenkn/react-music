using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Music.Models;
using Music.Repositories;
using Music.Services;
using Newtonsoft.Json.Linq;

namespace Music.Controllers
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

        [HttpPost]
        public Task<TrackPermissions> Save(Post req) => _trackService.Save(req.Tracks);
    }

    public class Post
    {
        public IEnumerable<TrackUserProps> Tracks { get; set; }
    }
}
