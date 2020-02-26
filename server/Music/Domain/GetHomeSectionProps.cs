using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.Domain.QueryTracksViaYoutube;
using Music.Domain.Shared;
using Newtonsoft.Json;
using Utilities;

namespace Music.Domain
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
            var user = await Db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (string.IsNullOrEmpty(user.HomeSectionStateJson))
            {
                var options = HomeSectionOptionsModel.CreateInitial();
                var tracks = await Resolve<QueryTracksExecutor>().Execute(options.Tracklist.QueryForm.MusicDbQuery);
                return new HomeSectionProps
                {
                    Options = options,
                    TracksFromMusicDb = tracks,
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

            if (queryForm.MusicDbQuery != null)
                props.TracksFromMusicDb = await Resolve<QueryTracksExecutor>().Execute(queryForm.MusicDbQuery);
            else
                props.TracksFromYouTube = await Resolve<QueryTracksViaYoutubeExecutor>().Execute(queryForm.YoutubeQuery);

            return props;
        }

        private async Task<IEnumerable<string>> GetAllTags()
        {
            var tags = await Db.TrackUserProps.SelectMany(tp => tp.TrackTags.Select(tt => tt.Value)).Distinct().ToArrayAsync();
            return tags;
        }

        private async Task<IEnumerable<IdWithName>> GetAllChannels()
        {
            var channels = await Db.YouTubeChannels.Select(c => new IdWithName {Id = c.Id, Name = c.Title}).ToArrayAsync();
            return channels;
        }
    }
}
