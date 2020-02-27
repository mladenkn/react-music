﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Music.Domain;
using Music.Domain.Shared;
using Music.Domain.Shared.Models;

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
        public Task SaveOptions(HomeSectionPersistableStateModel opt) => Resolve<HomeSectionOptionsRequests>().Save(opt);

        [HttpGet("props")]
        public Task<HomeSectionProps> GetProps() => Resolve<GetHomeSectionProps>().Execute();
    }
}
