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
        public Task<AdminSectionParams> Get() => Resolve<AdminService>().GetAdminSectionParams();

        [HttpPost("admin-section")]
        public Task Post(AdminSectionState s) => Resolve<AdminService>().SaveSectionState(s);

        [HttpPost]
        public Task<AdminCommandForAdminSection> Post(AdminCommandForAdminSection cmd) => Resolve<AdminService>().Add(cmd);

        [HttpPut]
        public Task Put(AdminCommandForAdminSection cmd) => Resolve<AdminService>().Update(cmd);

        [HttpPost("exe-command")]
        public Task<string> ExecuteCommand(Dictionary<string, string> args) => 
            Resolve<AdminCommandExecutor>().ExecuteComamnd(args["commandYaml"]);
    }
}
