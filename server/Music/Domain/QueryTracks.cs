using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kernel;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Models;
using Music.Domain.Shared;
using Utilities;

namespace Music.Domain
{
    public class QueryTracksExecutor : ServiceResolverAware
    {
        public QueryTracksExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ArrayWithTotalCount<TrackModel>> Execute(QueryTracksRequest req)
        {
            var query = BuildFilter(req);

            if (req.Randomize)
            {
                var allIds = await query.Select(t => t.YoutubeVideoId).ToArrayAsync();
                var chosenIds = allIds.Randomize().Take(req.Take);
                var newQuery = Db.TrackUserProps.Where(t => chosenIds.Contains(t.YoutubeVideoId));
                var result = await ExecuteQuery(newQuery, req);
                return result;
            }
            else
            {
                var result = await ExecuteQuery(query, req);
                return result;
            }
        }

        private async Task<ArrayWithTotalCount<TrackModel>> ExecuteQuery(IQueryable<TrackUserProps> query, QueryTracksRequest req)
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

        private IQueryable<TrackUserProps> BuildFilter(QueryTracksRequest req)
        {
            var userId = Resolve<ICurrentUserContext>().Id;

            var query = Db.TrackUserProps.Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(req.TitleContains))
                query = query.Where(t => t.YoutubeVideo.Title.Contains(req.TitleContains));

            if (!string.IsNullOrEmpty(req.YoutubeChannelId))
                query = query.Where(t => t.YoutubeVideo.YoutubeChannelId == req.YoutubeChannelId);

            if (req.MustHaveAnyTag != null && req.MustHaveAnyTag.Count > 0)
                query = query.Where(t => t.TrackTags.Any(tt => req.MustHaveAnyTag.Contains(tt.Value)));

            //var mustHaveEveryTag = req.MustHaveEveryTag?.ToArray();
            //if (mustHaveEveryTag != null && mustHaveEveryTag.Length > 0)
            //    query = query.Where(t => t.TrackTags
            //                                 .Select(tt => tt.Value)
            //                                 .Except(mustHaveEveryTag)
            //                                 .Count() == mustHaveEveryTag.Length
            //                        );

            //var mustHaveEveryTag = req.MustHaveEveryTag?.ToArray();
            //if (mustHaveEveryTag != null && mustHaveEveryTag.Length > 0)
            //    query = query.Where(t => mustHaveEveryTag.All(requiredTag => t.TrackTags.Any(tt => tt.Value == requiredTag)));

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
