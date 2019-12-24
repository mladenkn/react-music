using System;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess;

namespace Executables
{
    public enum DatabaseType
    {
        InMemory, SqlServer
    }

    public class Services : IDisposable
    {
        public MusicDbContext DbContext { get; }

        public Services(DatabaseType databaseType)
        {
            var dbOptionsBuilder = new DbContextOptionsBuilder<MusicDbContext>();

            if (databaseType == DatabaseType.SqlServer)
                dbOptionsBuilder.UseSqlServer(
                    "Data Source=DESKTOP-VSBO5TE\\SQLEXPRESS;Initial Catalog=MusicAutomatedTests;Integrated Security=True");
            else
                dbOptionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            DbContext = new MusicDbContext(dbOptionsBuilder.Options);
        }

        public void Dispose()
        {
            DbContext?.Dispose();
        }
    }
}
