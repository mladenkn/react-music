using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Music.App.Models;
using Music.App.Requests;

namespace Music.App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeSectionController : ServiceResolverAware
    {
        public HomeSectionController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost]
        public Task SaveOptions(HomeSectionPersistableStateModel opt) => Resolve<SaveHomeSectionOptions>().Save(opt);

        [HttpGet("props")]
        public Task<HomeSectionProps> GetProps() => Resolve<GetHomeSectionProps>().Execute();
    }
}
