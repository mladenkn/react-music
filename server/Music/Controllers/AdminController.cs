using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Music.Models;
using Music.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utilities;

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
        public Task<CsCommandForAdminSection> Post(CsCommandForAdminSection cmd) => Resolve<AdminService>().Add(cmd);

        [HttpPut("commands")]
        public Task Put(CsCommandForAdminSection cmd) => Resolve<AdminService>().Update(cmd);

        [HttpPost("commands/execute")]
        public Task<object> ExecuteCommand([FromBody] JObject args)
        {
            var dictParams = args.ToDictionary();
            return Resolve<AdminService>().ExecuteCommand(dictParams);
        }

        [HttpPost("cs-commands/execute")]
        public Task<object> ExecuteCommand([FromBody] dynamic args) => Resolve<AdminService>().ExecuteCsCommand((string) args.command);

        [HttpPost("variables")]
        public Task Post([FromBody] dynamic args) => Resolve<AdminService>().SetVariable((string) args.key, args.value);
    }
}
