using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Utilities;

namespace Kernel
{
    public class DataPersistor : ServiceResolverAware<DbContext>
    {
        private readonly DbContext _db;

        public DataPersistor(DbContext db, IServiceProvider sp) : base(sp)
        {
            _db = db;
        }

        public async Task Persist(Action<DataPersistorOperations> specifyOperations)
        {
            var actions = new List<Action>();
            var ops = new DataPersistorOperations(_db, actions.Add);
            specifyOperations(ops);
            await _db.SaveChangesAsync();
            actions.ForEach(a => a());
        }
    }

    public class DataPersistorOperations
    {
        private readonly DbContext _db;
        private readonly Action<Action> _afterCommitTransaction;

        public DataPersistorOperations(DbContext db, Action<Action> afterCommitTransaction)
        {
            _db = db;
            _afterCommitTransaction = afterCommitTransaction;
        }

        public void Add<TEntity>(TEntity entity) 
            where TEntity: IDatabaseEntity, new()
        {
            _db.Add(entity);
        }

        public void Add<TEntity>(TEntity entity, Action<TEntity> mutate, Action<TEntity, TEntity> setId)
            where TEntity : IDatabaseEntity, new()
        {
            var entityCopy = ReflectionUtils.ShallowCopy(entity);
            mutate(entityCopy);
            _db.Add(entityCopy);
            _afterCommitTransaction(() => setId(entity, entityCopy));
        }

        public void Update<TEntity>(TEntity entity)
            where TEntity : IDatabaseEntity, new()
        {
            _db.Update(entity);
        }

        public void Update<TEntity>(TEntity entity, Action<TEntity> mutate, Action<TEntity, TEntity> setId)
            where TEntity : IDatabaseEntity, new()
        {
            var entityCopy = ReflectionUtils.ShallowCopy(entity);
            mutate(entityCopy);
            _db.Update(entityCopy);
            _afterCommitTransaction(() => setId(entity, entityCopy));
        }

        public void Remove<TEntity>(TEntity entity) 
            where TEntity : IDatabaseEntity, new()
        {
            _db.Remove(entity);
        }
    }
}
