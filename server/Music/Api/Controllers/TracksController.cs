using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kernel;
using Microsoft.AspNetCore.Mvc;
using Music.DataAccess;
using Music.Domain;
using Music.Domain.QueryTracksViaYoutube;
using Music.Domain.Shared;
using Utilities;

namespace Music.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracksController : ServiceResolverAware<MusicDbContext>
    {
        public TracksController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public async Task<object> Get([FromQuery] string youTubeSearchQuery = null, [FromQuery]QueryTracksRequest dbSourceParams = null)
        {
            if (youTubeSearchQuery != null)
            {
                var r = await Resolve<QueryTracksViaYoutubeExecutor>().Execute(youTubeSearchQuery);
                return r.Select(TrackModelToClientModel);
            }
            else if (dbSourceParams != null)
            {
                var r = await Resolve<QueryTracksExecutor>().Execute(dbSourceParams);
                var data = r.Data.Select(TrackModelToClientModel).ToArray();
                return new ArrayWithTotalCount<dynamic>(data, r.TotalCount);
            }
            else
                throw new Exception();
        }

        [HttpPost]
        public Task Save([FromBody]SaveTrackModel trackProps) => 
            Resolve<SaveTrackYoutubeExecutor>().Execute(trackProps);

        private dynamic TrackModelToClientModel(TrackModel track) =>
            new
            {
                YtId = track.YoutubeVideoId,
                track.Title,
                track.Description,
                track.Image,
                track.Year,
                Genres = new string[0],
                track.Tags,
                Channel = new
                {
                    Id = track.YoutubeChannelId,
                    Title = track.YoutubeChannelTitle,
                }
            };
    }
}
