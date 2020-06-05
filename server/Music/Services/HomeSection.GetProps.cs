using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.DbModels;
using Music.Models;
using Newtonsoft.Json;
using Utilities;

namespace Music.Services
{
    public class HomeSectionProps
    {
        public HomeSectionOptionsModel Options { get; set; }

        public string SelectedTrackYoutubeId { get; set; }

        public string CurrentTrackYoutubeId { get; set; }

        public ArrayWithTotalCount<TrackForHomeSection> TracksFromMusicDb { get; set; }

        public IEnumerable<TrackForHomeSection> TracksFromYouTube { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<IdWithName> YouTubeChannels { get; set; }
    }

    public partial class HomeSectionService : ServiceResolverAware
    {
        public HomeSectionService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<HomeSectionProps> GetProps()
        {
            var userId = Resolve<ICurrentUserContext>().Id;
            var user = await Query<User>().FirstOrDefaultAsync(u => u.Id == userId);

            var tracksService = Resolve<TracksService>();

            if (string.IsNullOrEmpty(user.HomeSectionStateJson))
            {
                var options = HomeSectionOptionsModel.CreateInitial();
                var tracks = await tracksService.QueryMusicDb(options.Tracklist.Query.MusicDbQuery);
                return new HomeSectionProps
                {
                    Options = options,
                    TracksFromMusicDb = tracks,
                    Tags = await GetAllTags(),
                    YouTubeChannels = await GetAllChannels(),
                };
            }

            var homeSectionPersistableState = JsonConvert.DeserializeObject<HomeSectionPersistableStateModel>(user.HomeSectionStateJson);

            var props = new HomeSectionProps
            {
                Options = homeSectionPersistableState.Options,
                CurrentTrackYoutubeId = homeSectionPersistableState.CurrentTrackYoutubeId,
                SelectedTrackYoutubeId = homeSectionPersistableState.SelectedTrackYoutubeId,
                Tags = await GetAllTags(),
                YouTubeChannels = await GetAllChannels(),
            };

            var queryForm = homeSectionPersistableState.Options.Tracklist.Query;

            if (queryForm.Type == "MusicDbQuery")
                props.TracksFromMusicDb = await tracksService.QueryMusicDb(queryForm.MusicDbQuery);
            else if (queryForm.Type == "YouTubeQuery")
                props.TracksFromYouTube = await tracksService.QueryViaYouTube(queryForm.YouTubeQuery);
            else
                throw new Exception("Unallowed value.");

            return props;
        }

        private async Task<IEnumerable<string>> GetAllTags()
        {
            var tags = await Query<TrackUserProps>().SelectMany(tp => tp.TrackTags.Select(tt => tt.Value)).Distinct().ToArrayAsync();
            return tags;
        }

        private async Task<IEnumerable<IdWithName>> GetAllChannels()
        {
            var channels = await Query<YouTubeChannel>().Select(c => new IdWithName { Id = c.Id, Name = c.Title }).ToArrayAsync();
            return channels;
        }
    }
}
