using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Utilities;

namespace Kernel
{
    public static class QueryableUtils
    {
        public static async Task<ArrayWithTotalCount<TQueryProjectedModel>> ToArrayWithTotalCount<TQueryModel,
            TQueryProjectedModel>(
            this IQueryable<TQueryModel> query,
            Func<IQueryable<TQueryModel>, IQueryable<TQueryProjectedModel>> upgradeQuery
        )
        {
            var list = await upgradeQuery(query).ToArrayAsync();
            var count = await query.CountAsync();
            return new ArrayWithTotalCount<TQueryProjectedModel>(list, count);
        }
    }
}
