﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.DbModels;
using Music.App.Models;
using Utilities;

namespace Music.App.Services
{
    public class SaveTrackModel
    {
        public long TrackId { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }

        public TracksQueryModel Query { get; set; }
    }

    public class SaveTrackExecutor : ServiceResolverAware
    {
        public SaveTrackExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ArrayWithTotalCount<TrackModel>> Execute(SaveTrackModel req)
        {
            var currentUserId = Resolve<ICurrentUserContext>().Id;

            var trackUserProps = await Query<TrackUserProps>()
                .FirstOrDefaultAsync(t => t.TrackId == req.TrackId && t.UserId == currentUserId);

            var newTags = req.Tags
                .Select(t => new TrackUserPropsTag { TrackUserPropsId = trackUserProps?.Id ?? 0, Value = t })
                .ToArray();

            if (trackUserProps != null)
            {
                if (currentUserId != trackUserProps.UserId)
                    throw new ApplicationException("Trying to update other users track.");

                trackUserProps.Year = req.Year;
                var tagsToDelete = await Db.TrackUserPropsTags.Where(t => t.TrackUserPropsId == trackUserProps.Id).ToArrayAsync();

                await Persist(ops =>
                {
                    ops.DeleteTrackUserPropsTags(tagsToDelete);
                });

                await Persist(ops =>
                {
                    ops.InsertTrackUserPropsTags(newTags);
                    ops.UpdateTrackUserProps(new []{ trackUserProps });
                });
            }
            else
            {
                var track = await Query<Track>()
                    .Include(t => t.YoutubeVideos)
                    .FirstOrDefaultAsync(t => t.Id == req.TrackId);

                if(track == null)
                    throw new ApplicationException("Track not found.");

                var newTrackProps = new TrackUserProps
                {
                    TrackTags = newTags,
                    UserId = currentUserId,
                    TrackId = req.TrackId,
                    Year = req.Year,
                    InsertedAt = DateTime.Now,
                    YoutubeVideoId = track.YoutubeVideos.First().Id
                };

                await Persist(ops => { ops.InsertTrackUserProps(new []{ newTrackProps }); });
            }

            if (req.Query != null)
                return await Resolve<QueryTracksExecutor>().Execute(req.Query);
            else
                return null;
        }
    }
}