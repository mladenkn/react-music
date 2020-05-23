using System;
using System.Linq;
using System.Threading.Tasks;
using Kernel;
using Music.App.DbModels;
using Music.App.Models;
using Utilities;

namespace Music.App.Services
{
    public partial class TracksService : ServiceResolverAware
    {
        public TracksService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ArrayWithTotalCount<TrackForHomeSection>> QueryMusicDb(MusicDbTrackQueryParamsModel req)
        {
            var query = BuildFilterNew(req);
            var userId = Resolve<ICurrentUserContext>().Id;

            if (req.Randomize)
            {
                var result = await query
                    .OrderBy(t => Guid.NewGuid())
                    .Select(TrackForHomeSection.FromTrack(userId))
                    .ToArrayWithTotalCount(q =>
                        q.Take(req.Take)
                    );
                return result;
            }
            else
            {
                var result = await query
                    .Select(TrackForHomeSection.FromTrack(userId))
                    .ToArrayWithTotalCount(q => q
                        .Skip(req.Skip)
                        .Take(req.Take)
                    );
                return result;
            }
        }

        private IQueryable<Track> BuildFilterNew(MusicDbTrackQueryParamsModel req)
        {
            var userId = Resolve<ICurrentUserContext>().Id;

            var query = Query<Track>();

            if (!string.IsNullOrEmpty(req.TitleContains))
                query = query.Where(t => t.YoutubeVideos.First().Title.Contains(req.TitleContains));

            if (req.SupportedYouTubeChannelsIds != null && req.SupportedYouTubeChannelsIds.Any())
                query = query.Where(t => req.SupportedYouTubeChannelsIds.Contains(t.YoutubeVideos.First().YoutubeChannelId));

            if (req.MustHaveAnyTag != null && req.MustHaveAnyTag.Any())
                query = query.Where(t => t.TrackUserProps.FirstOrDefault(p => p.UserId == userId).TrackTags.Any(tt => req.MustHaveAnyTag.Contains(tt.Value)));

            //var mustHaveEveryTag = req.MustHaveEveryTag?.ToArray();
            //if (mustHaveEveryTag != null && mustHaveEveryTag.Length > 0)
            //    query = query.Where(t => t.TrackUserPropsTags
            //                                 .Select(tt => tt.Value)
            //                                 .Except(mustHaveEveryTag)
            //                                 .Count() == mustHaveEveryTag.Length
            //                        );

            //var mustHaveEveryTag = req.MustHaveEveryTag?.ToArray();
            //if (mustHaveEveryTag != null && mustHaveEveryTag.Length > 0)
            //    query = query.Where(t => mustHaveEveryTag.All(requiredTag => t.TrackUserPropsTags.Any(tt => tt.Value == requiredTag)));

            if (req.MustHaveEveryTag != null)
            {
                foreach (var reqTag in req.MustHaveEveryTag)
                    query = query.Where(t => t.TrackUserProps.FirstOrDefault(p => p.UserId == userId).TrackTags.Any(tt => tt.Value == reqTag));
            }

            if (req.YearRange != null)
            {
                if (req.YearRange.LowerBound > 0)
                    query = query.Where(t => t.TrackUserProps.FirstOrDefault(p => p.UserId == userId).Year >= req.YearRange.LowerBound);

                if (req.YearRange.UpperBound > 0)
                    query = query.Where(t => t.TrackUserProps.FirstOrDefault(p => p.UserId == userId).Year < req.YearRange.UpperBound);
            }

            return query;
        }
    }
}
