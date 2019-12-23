using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess;

namespace Executables
{
    public class Services
    {
        public MusicDbContext DbContext { get; }

        public Services()
        {
            var dbOptions = new DbContextOptionsBuilder<MusicDbContext>()
                .UseSqlServer("Data Source=DESKTOP-VSBO5TE\\SQLEXPRESS;Initial Catalog=MusicAutomatedTests;Integrated Security=True")
                .Options;
            DbContext = new MusicDbContext(dbOptions);
        }
    }
}
