using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using Music.App.Models;
using Newtonsoft.Json;
using Utilities;

namespace Music.App.Services
{
    public class HomeSectionProps
    {
        public HomeSectionOptionsModel Options { get; set; }

        public string SelectedTrackYoutubeId { get; set; }

        public string CurrentTrackYoutubeId { get; set; }

        public ArrayWithTotalCount<TrackModel> TracksFromMusicDb { get; set; }

        public IEnumerable<TrackModel> TracksFromYouTube { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<IdWithName> YouTubeChannels { get; set; }
    }

    public class GetHomeSectionProps : ServiceResolverAware
    {
        public GetHomeSectionProps(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<HomeSectionProps> Execute()
        {
            var userId = Resolve<ICurrentUserContext>().Id;
            var user = await Query<User>().FirstOrDefaultAsync(u => u.Id == userId);

            if (string.IsNullOrEmpty(user.HomeSectionStateJson))
            {
                var options = HomeSectionOptionsModel.CreateInitial();
                var tracks = await Resolve<QueryTracksExecutor>().Execute(options.Tracklist.QueryForm.MusicDbQuery);
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

            var queryForm = homeSectionPersistableState.Options.Tracklist.QueryForm;

            if (queryForm.DataSource == "MusicDb")
                props.TracksFromMusicDb = await Resolve<QueryTracksExecutor>().Execute(queryForm.MusicDbQuery);
            else
                props.TracksFromYouTube = await Resolve<QueryTracksViaYoutubeExecutor>().Execute(queryForm.YouTubeQuery);

            return props;
        }

        private async Task<IEnumerable<string>> GetAllTags()
        {
            var tags = await Query<TrackUserProps>().SelectMany(tp => tp.TrackTags.Select(tt => tt.Value)).Distinct().ToArrayAsync();
            return tags;
        }

        private async Task<IEnumerable<IdWithName>> GetAllChannels()
        {
            var channels = await Query<YouTubeChannel>().Select(c => new IdWithName {Id = c.Id, Name = c.Title}).ToArrayAsync();
            return channels;
        }
    }
}
