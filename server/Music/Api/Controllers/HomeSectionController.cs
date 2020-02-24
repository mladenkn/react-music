using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Music.DataAccess.Models;
using Music.Domain;

namespace Music.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeSectionController : ServiceResolverAware
    {
        public HomeSectionController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost]
        public Task SaveOptions(HomeSectionOptions opt) =>Resolve<HomeSectionOptionsRequests>().Save(opt);
    }
}
