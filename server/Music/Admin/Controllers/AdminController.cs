using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Music.Admin.Models;
using Music.Admin.Services;

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
        public Task<AdminSectionParams> Get() => Resolve<AdminCommandsService>().GetAdminSectionParams();

        [HttpPost]
        public Task Post(AdminCommandForAdminSection cmd) => Resolve<AdminCommandsService>().Add(cmd);

        [HttpPut]
        public Task Put(AdminCommandForAdminSection cmd) => Resolve<AdminCommandsService>().Update(cmd);
    }
}
