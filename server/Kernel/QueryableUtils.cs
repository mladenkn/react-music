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
            var listTask = upgradeQuery(query).ToArrayAsync();
            var countTask = query.CountAsync();

            await Task.WhenAll(listTask, countTask);

            var list = await listTask;
            var count = await countTask;

            return new ArrayWithTotalCount<TQueryProjectedModel>(list, count);
        }
    }
}
