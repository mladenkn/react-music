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
            var query = Db.Tracks.AsQueryable();

            if (req.MustHaveAnyTag != null)
                query = query.Where(track => track.TrackTags.Any(trackTag => req.MustHaveAnyTag.Contains(trackTag.Value)));

            if (req.MustHaveEveryTag != null)
                query = query.Where(track => track.TrackTags.All(trackTag => req.MustHaveAnyTag.Contains(trackTag.Value)));

            if (req.YearRange != null)
                query = query.Where(track => track.Year >= req.YearRange.LowerBound &&
                                             track.Year < req.YearRange.UpperBound);

            var result = await query.ToArrayWithTotalCount(
                q => q
                    .Skip(req.Skip)
                    .Take(req.Take)
                    .ProjectTo<TrackModel>(Mapper.ConfigurationProvider)
            );

            return result;
        }
    }
}
