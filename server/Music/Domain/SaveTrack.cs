using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kernel;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess;
using Music.DataAccess.Models;
using Music.Domain.Shared;

namespace Music.Domain
{
    public class SaveTrackModel
    {
        public string TrackYtId { get; set; }

        public int? Year { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }
    }

    public class SaveTrackYoutubeExecutor : ServiceResolverAware<MusicDbContext>
    {
        public SaveTrackYoutubeExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task Execute(SaveTrackModel saveModel)
        {
            var track = await Db.Tracks
                .FirstOrDefaultAsync(t => t.YoutubeVideoId == saveModel.TrackYtId);

            var currentUserContext = Resolve<ICurrentUserContext>();

            var newTags = saveModel.Tags
                .Select(t => new TrackTag { TrackId = track?.Id ?? 0, Value = t })
                .ToArray();

            if (track != null)
            {
                if (currentUserContext.Id != track.UserId)
                    throw new ApplicationException("Trying to update other users track.");

                track.Year = saveModel.Year;
                Db.Tracks.Update(track);

                var tagsToDelete = await Db.TrackTags.Where(t => t.TrackId == track.Id).ToArrayAsync();
                Db.TrackTags.RemoveRange(tagsToDelete);

                Db.TrackTags.AddRange(newTags);

                await Db.SaveChangesAsync();
            }
            else
            {
                var newTrack = new Track
                {
                    TrackTags = newTags,
                    UserId = currentUserContext.Id,
                    Year = saveModel.Year,
                    YoutubeVideoId = saveModel.TrackYtId,
                };
                Db.Add(newTrack);
                await Db.SaveChangesAsync();
            }
        }
    }
}
