﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using Music.App.Models;
using Utilities;

namespace Music.App
{
    public class DataPersistor : ServiceResolverAware
    {
        private readonly MusicDbContext _db;

        public DataPersistor(MusicDbContext db, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _db = db;
        }

        public async Task InsertYoutubeVideos(IEnumerable<YoutubeVideo> videos)
        {
            var videosReadyToInsert = videos.Select(v =>
            {
                var copy = ReflectionUtils.ShallowCopy(v);
                copy.YouTubeChannel = null;
                return copy;
            });

            var allChannels = videos.Select(v => v.YouTubeChannel).DistinctBy(c => c.Id);
            var allChannelsIdsFromDb = await _db.Set<YouTubeChannel>().Select(c => c.Id).ToArrayAsync();
            var channelsReadyToInsert = allChannels.Where(c => !c.Id.IsIn(allChannelsIdsFromDb));

            _db.AddRange(videosReadyToInsert);
            _db.AddRange(channelsReadyToInsert);

            await _db.SaveChangesAsync();
        }
    }
}