using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Kernel;
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

        public IEnumerable<string> MustHaveEveryTag { get; set; }

        public IEnumerable<string> MustHaveAnyTag { get; set; }

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

            if (req.TitleContains != null)
                query = query.Where(t => t.YoutubeVideo.Title.Contains(req.TitleContains));

            if(req.YoutubeChannelId != null)
                query = query.Where(t => t.YoutubeVideo.YoutubeChannelId == req.YoutubeChannelId);

            if (req.MustHaveAnyTag != null)
                query = query.Where(t => t.TrackTags.Any(trackTag => req.MustHaveAnyTag.Contains(trackTag.Value)));

            if (req.MustHaveEveryTag != null)
                query = query.Where(t => t.TrackTags.All(trackTag => req.MustHaveEveryTag.Contains(trackTag.Value)));

            if (req.YearRange != null)
                query = query.Where(t => t.Year >= req.YearRange.LowerBound &&
                                             t.Year < req.YearRange.UpperBound);

            var result = await query
                .Select(TrackModel.FromTrackUserProps)
                .ToArrayWithTotalCount(q => q
                    .Skip(req.Skip)
                    .Take(req.Take)
                );

            return result;
        }
    }
}
