using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Music.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracksController : ControllerBase
    {
        [HttpGet]
        public Task Get() => throw new NotImplementedException();

        [HttpGet("yt")]
        public Task SearchYoutube() => throw new NotImplementedException();

        [HttpPost]
        public Task Save() => throw new NotImplementedException();
    }
}
