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
        private readonly YoutubeDataApiVideoRepository _repo;

        public YoutubeVideoMasterRepository(YoutubeDataApiVideoRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<YoutubeVideo>> 
            GetList(IReadOnlyCollection<string> ids) =>
            _repo.GetList(ids);
    }
}
