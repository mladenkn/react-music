using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music.Models;

namespace Music.Repositories
{
    public class YoutubeVideoMasterRepository
    {
        public async Task<IEnumerable<YoutubeVideo>> 
            GetList(IEnumerable<string> ids)
        {
            throw new NotImplementedException();
        }
    }
}
