using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Music.Operations;
using Music.Repositories;
using Music.Services;

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
    }
}
