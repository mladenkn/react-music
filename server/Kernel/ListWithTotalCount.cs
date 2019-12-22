using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Utilities;

namespace Kernel
{
    public static class ListWithTotalCount
    {
        public static async Task<ListWithTotalCount<TQueryProjectedModel>> FromQuery<TQueryModel, TQueryProjectedModel>(
            IQueryable<TQueryModel> query, Func<IQueryable<TQueryModel>, IQueryable<TQueryProjectedModel>> upgradeQuery
            )
        {
            var listTask = upgradeQuery(query).ToArrayAsync();
            var countTask = query.CountAsync();

            await Task.WhenAll(listTask, countTask);

            var list = await listTask;
            var count = await countTask;
            
            return new ListWithTotalCount<TQueryProjectedModel>(list, count);
        }
    }
}
