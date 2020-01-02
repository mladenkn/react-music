using System;
using System.Threading.Tasks;
using Music.DataAccess;

namespace Executables.Helpers
{
    public class TestDatabaseClient : IAsyncDisposable
    {
        public string DatabaseName { get; }
        private MusicDbContext _db;

        private TestDatabaseClient(string databaseName, MusicDbContext db)
        {
            DatabaseName = databaseName;
            _db = db;
        }

        public async Task UseIt(params Func<MusicDbContext, Task>[] actions)
        {
            foreach (var action in actions)
            {
                _db = Utils.UseDatabase(DatabaseName);
                await action(_db);
            }
        }

        public static async Task<TestDatabaseClient> Create()
        {
            var dbName = Guid.NewGuid().ToString();
            var db = Utils.UseDatabase(dbName);
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
            return new TestDatabaseClient(dbName, db);
        }

        public async ValueTask DisposeAsync()
        {
            await _db.Database.EnsureDeletedAsync();
            await _db.DisposeAsync();
        }
    }
}
