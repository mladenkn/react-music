using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.App.Models;
using Utilities;

namespace Music.App.Requests
{
    public class SaveTrackRequest
    {
        public string TrackYtId { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }

        public TracksQueryModel Query { get; set; }
    }

    public class SaveTrackExecutor : ServiceResolverAware
    {
        public SaveTrackExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ArrayWithTotalCount<TrackModel>> Execute(SaveTrackRequest req)
        {
            var track = await Db.TrackUserProps
                .FirstOrDefaultAsync(t => t.YoutubeVideoId == req.TrackYtId);

            var currentUserContext = Resolve<ICurrentUserContext>();

            var newTags = req.Tags
                .Select(t => new TrackUserPropsTag { TrackUserPropsId = track?.Id ?? 0, Value = t })
                .ToArray();

            if (track != null)
            {
                if (currentUserContext.Id != track.UserId)
                    throw new ApplicationException("Trying to update other users track.");

                track.Year = req.Year;
                Db.TrackUserProps.Update(track);

                var tagsToDelete = await Db.TrackTags.Where(t => t.TrackUserPropsId == track.Id).ToArrayAsync();
                Db.TrackTags.RemoveRange(tagsToDelete);

                Db.TrackTags.AddRange(newTags);

                await Db.SaveChangesAsync();
            }
            else
            {
                var newTrack = new TrackUserProps
                {
                    TrackTags = newTags,
                    UserId = currentUserContext.Id,
                    Year = req.Year,
                    YoutubeVideoId = req.TrackYtId,
                    InsertedAt = DateTime.Now
                };
                Db.Add(newTrack);
                await Db.SaveChangesAsync();
            }

            if (req.Query != null)
                return await Resolve<QueryTracksExecutor>().Execute(req.Query);
            else
                return null;
        }
    }
}
