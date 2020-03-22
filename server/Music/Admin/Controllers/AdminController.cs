using System;
using System.Collections.Generic;
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

        [HttpGet("admin-section")]
        public Task<AdminSectionParams> Get() => Resolve<AdminSectionService>().GetAdminSectionParams();

        [HttpPost("admin-section")]
        public Task Post(AdminSectionState s) => Resolve<AdminSectionService>().SaveSectionState(s);

        [HttpPost]
        public Task<AdminCommandForAdminSection> Post(AdminCommandForAdminSection cmd) => Resolve<AdminSectionService>().Add(cmd);

        [HttpPut]
        public Task Put(AdminCommandForAdminSection cmd) => Resolve<AdminSectionService>().Update(cmd);

        [HttpPost("exe-command")]
        public Task<string> ExecuteCommand(Dictionary<string, string> args) => 
            Resolve<AdminCommandExecutor>().ExecuteCommand(args["commandYaml"]);
    }
}
