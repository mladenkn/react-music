using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Music.Models;
using Music.Services;
using Newtonsoft.Json.Linq;

namespace Music.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        public AdminController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet("section/props")]
        public Task<AdminSectionParams> Get() => Resolve<AdminService>().GetAdminSectionParams();

        [HttpPost("section/props")]
        public Task Post(AdminSectionState s) => Resolve<AdminService>().SaveSectionState(s);

        [HttpPost("commands")]
        public Task<AdminCommandForAdminSection> Post(AdminCommandForAdminSection cmd) => Resolve<AdminService>().Add(cmd);

        [HttpPut("commands")]
        public Task Put(AdminCommandForAdminSection cmd) => Resolve<AdminService>().Update(cmd);

        [HttpPost("commands/execute")]
        public Task<object> ExecuteCommand([FromBody] JObject args) => Resolve<AdminCommandExecutor>().ExecuteCommand(args);

        [HttpPost("variables")]
        public Task Post(string key, object value) => Resolve<AdminService>().SetVariable(key, value);
    }
}
