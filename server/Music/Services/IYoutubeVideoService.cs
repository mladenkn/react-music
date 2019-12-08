using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music.Models;

namespace Music.Services
{
    public interface IYoutubeVideoService
    {
        Task<IEnumerable<YoutubeVideo>> GetList(IReadOnlyCollection<string> ids);
    }
}
