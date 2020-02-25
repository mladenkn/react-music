﻿using System;
using System.Collections.Generic;
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
                SelectedTrackYoutubeId = homeSectionPersistableState.SelectedTrackYoutubeId
            };

            var queryForm = homeSectionPersistableState.Options.Tracklist.QueryForm;

            if (queryForm.DataSource == "MusicDb")
                props.TracksFromMusicDb = await Resolve<QueryTracksExecutor>().Execute(queryForm.MusicDbQuery);
            else
                props.TracksFromYouTube = await Resolve<QueryTracksViaYoutubeExecutor>().Execute(queryForm.YouTubeQuery);

            return props;
        }
    }
}