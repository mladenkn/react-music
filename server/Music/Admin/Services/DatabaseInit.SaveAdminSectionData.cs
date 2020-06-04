using System.Threading.Tasks;
using Music.Admin.Models;
using Utilities;

namespace Music.Admin.Services
{
    public partial class DatabaseInit
    {
        public async Task SaveAdminSectionData()
        {
            var commands = new[]
            {
                new AdminCommand
                {
                    Name = "_",
                    Yaml = "",
                    UserId = 1,
                },
                new AdminCommand
                {
                    Name = "GetTracksWithoutYouTubeVideos",
                    Yaml = "type: GetTracksWithoutYouTubeVideos",
                    UserId = 1,
                },
                new AdminCommand
                {
                    Name = "GetVideosWithoutTracks",
                    Yaml = "type: GetVideosWithoutTracks",
                    UserId = 1,
                },
                new AdminCommand
                {
                    Name = "DeleteVideos",
                    Yaml = "type: DeleteVideos",
                    UserId = 1,
                },
                new AdminCommand
                {
                    Name = "DeleteTracks",
                    Yaml = "type: DeleteTracks",
                    UserId = 1,
                },
            };

            await Persist(ops => commands.ForEach(ops.Add));
        }
    }
}
