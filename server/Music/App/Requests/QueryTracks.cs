using System;
using System.Linq;
using System.Threading.Tasks;
using Kernel;
using Music.App.DbModels;
using Music.App.Models;
using Utilities;

namespace Music.App.Requests
{
    public class QueryTracksExecutor : ServiceResolverAware
    {
        public QueryTracksExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ArrayWithTotalCount<TrackModel>> Execute(TracksQueryModel req)
        {
            var query = BuildFilter(req);

            if (req.Randomize)
            {
                var result = await query
                    .OrderBy(t => Guid.NewGuid())
                    .Select(TrackModel.FromTrackUserProps)
                    .ToArrayWithTotalCount(q =>
                        q.Take(req.Take)
                    );
                return result;
            }
            else
            {
                var result = await query
                    .OrderByDescending(t => t.InsertedAt)
                    .Select(TrackModel.FromTrackUserProps)
                    .ToArrayWithTotalCount(q => q
                        .Skip(req.Skip)
                        .Take(req.Take)
                    );
                return result;
            }
        }

        private IQueryable<TrackUserProps> BuildFilter(TracksQueryModel req)
        {
            var userId = Resolve<ICurrentUserContext>().Id;

            var query = Query<TrackUserProps>().Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(req.TitleContains))
                query = query.Where(t => t.YoutubeVideo.Title.Contains(req.TitleContains));

            if (req.SupportedYouTubeChannelsIds != null && req.SupportedYouTubeChannelsIds.Any())
                query = query.Where(t => req.SupportedYouTubeChannelsIds.Contains(t.YoutubeVideo.YoutubeChannelId));

            if (req.MustHaveAnyTag != null && req.MustHaveAnyTag.Any())
                query = query.Where(t => t.TrackTags.Any(tt => req.MustHaveAnyTag.Contains(tt.Value)));

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
                    query = query.Where(t => t.TrackTags.Any(tt => tt.Value == reqTag));
            }

            if (req.YearRange != null)
            {
                if (req.YearRange.LowerBound > 0)
                    query = query.Where(t => t.Year >= req.YearRange.LowerBound);

                if (req.YearRange.UpperBound > 0)
                    query = query.Where(t => t.Year < req.YearRange.UpperBound);
            }

            return query;
        }
    }
}
