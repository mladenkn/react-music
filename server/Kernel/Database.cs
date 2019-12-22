using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Kernel
{
    public interface IDatabase
    {
        IQueryable<TModel> Query<TModel>() where TModel : class;

        void Add(object model);
        
        void AddRange(IEnumerable<object> model);

        void Update(object model);

        void UpdateRange(IEnumerable<object> model);

        void Delete(object model);

        void DeleteRange(IEnumerable<object> model);

        Task SaveChangesAsync();
    }

    public class Database : IDatabase
    {
        private readonly DbContext _dbContext;

        public Database(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TModel> Query<TModel>() where TModel : class => _dbContext.Set<TModel>();

        public void Add(object model) => _dbContext.Add(model);
        public void AddRange(IEnumerable<object> model) => _dbContext.AddRange(model);

        public void Update(object model) => _dbContext.Update(model);
        public void UpdateRange(IEnumerable<object> model) => _dbContext.UpdateRange(model);

        public void Delete(object model) => _dbContext.Remove(model);
        public void DeleteRange(IEnumerable<object> model) => _dbContext.RemoveRange(model);

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
