using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Kernel;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess;
using Music.Domain.Shared;
using Utilities;

namespace Music.Domain
{
    public class QueryTracksRequest
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public string TitleContains { get; set; }

        public string YoutubeChannelId { get; set; }

        public IReadOnlyCollection<string> MustHaveEveryTag { get; set; }

        public IReadOnlyCollection<string> MustHaveAnyTag { get; set; }

        public Range<int> YearRange { get; set; }
    }

    public class QueryTracksExecutor : ServiceResolverAware<MusicDbContext>
    {
        public QueryTracksExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ArrayWithTotalCount<TrackModel>> Execute(QueryTracksRequest req)
        {
            var userId = Resolve<ICurrentUserContext>().Id;

            var query = Db.TrackUserProps.Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(req.TitleContains))
                query = query.Where(t => t.YoutubeVideo.Title.Contains(req.TitleContains));

            if(!string.IsNullOrEmpty(req.YoutubeChannelId))
                query = query.Where(t => t.YoutubeVideo.YoutubeChannelId == req.YoutubeChannelId);

            //var mustHaveAnyTag = req.MustHaveAnyTag?.ToArray();
            //if (mustHaveAnyTag != null && mustHaveAnyTag.Length > 0)
            //    query = query.Where(t => t.TrackTags
            //                                 .Select(tt => tt.Value)
            //                                 .Except(mustHaveAnyTag)
            //                                 .Count() > 0
            //                         );

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
            
            //if (req.MustHaveEveryTag != null && req.MustHaveEveryTag.Count > 0)
            //    query = query.Where(t => t.TrackTags.Any(tt => tt.Value == req.MustHaveEveryTag.First()));

            if (req.YearRange != null)
            {
                if (req.YearRange.LowerBound > 0)
                    query = query.Where(t => t.Year >= req.YearRange.LowerBound);

                if (req.YearRange.UpperBound > 0)
                    query = query.Where(t => t.Year < req.YearRange.UpperBound);
            }

            var result = await query
                .Select(TrackModel.FromTrackUserProps)
                .ToArrayWithTotalCount(q => q
                    .Skip(req.Skip)
                    .Take(req.Take)
                );

            if (req.MustHaveEveryTag != null && req.MustHaveEveryTag.Count > 0)
            {
                var refilteredInMemory = result.Data.Where(track =>
                    req.MustHaveEveryTag.All(requestedTag => track.Tags.Contains(requestedTag))
                ).ToArray();
                var newTotalCount = result.TotalCount - result.Data.Length + refilteredInMemory.Length;
                return new ArrayWithTotalCount<TrackModel>(refilteredInMemory, newTotalCount);
            }
            else
                return result;
        }
    }
}
