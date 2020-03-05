using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = Music.App.ControllerBase;

namespace Music.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        public AdminController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<string> Get()
        {
            return "radi";
        }
    }
}
