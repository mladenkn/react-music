using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Music.Admin.Models;
using Music.Admin.Services;
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
        public Task<IEnumerable<AdminYamlCommand>> Get() => Resolve<AdminCommandsService>().Get();
    }
}
