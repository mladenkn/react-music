using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Kernel;
using Music.DataAccess.Models;
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

    public class QueryTracksExecutor : ServiceBase
    {
        public QueryTracksExecutor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<ArrayWithTotalCount<Track>> Execute(QueryTracksRequest req)
        {
            var query = Db.Set<TrackUserPropsDbModel>().AsQueryable();

            if (req.MustHaveAnyTag != null)
                query = query.Where(track => track.Tags.Any(trackTag => req.MustHaveAnyTag.Contains(trackTag)));

            if (req.MustHaveEveryTag != null)
                query = query.Where(track => track.Tags.All(trackTag => req.MustHaveAnyTag.Contains(trackTag)));

            if (req.YearRange != null)
                query = query.Where(track => track.Year >= req.YearRange.LowerBound &&
                                             track.Year < req.YearRange.UpperBound);

            var result = await query.ToArrayWithTotalCount(
                q => q
                    .Skip(req.Skip)
                    .Take(req.Take)
                    .ProjectTo<Track>(Mapper.ConfigurationProvider)
            );

            return result;
        }
    }
}
