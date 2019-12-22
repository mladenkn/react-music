using System;
using System.Threading.Tasks;
using Kernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Music.Domain;
using Music.Domain.QueryTracksViaYoutube;
using Music.Domain.Shared;
using Utilities;

namespace Music.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracksController : ServiceResolverAware
    {
        public TracksController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public Task<ArrayWithTotalCount<Track>> Get(QueryTracksRequest r) => Resolve<QueryTracksExecutor>().Execute(r);

        [HttpGet("yt")]
        public Task QueryTracksViaYoutube(string searchQuery) => Resolve<QueryTracksViaYoutubeExecutor>().Execute(searchQuery);

        [HttpPost]
        public Task Save(TrackUserProps trackProps) => Resolve<SaveTrackYoutubeExecutor>().Execute(trackProps);
    }
}
