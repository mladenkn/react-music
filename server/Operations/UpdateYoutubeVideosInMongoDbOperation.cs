using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Executables
{
    public class UpdateYoutubeVideosInMongoDbOperation
    {
        //[Fact]
        public async Task Run()
        {
            var services = new Services();

            var storedVideosIds = await services.YoutubeVideoMongoRepository.GetAllIds();
            var storedVideosUpdated = await services.YoutubeDataApiVideoRepository.GetList(storedVideosIds.ToArray());

            var videosJson = JsonConvert.SerializeObject(storedVideosUpdated, Formatting.Indented);
            await Helpers.WriteToFile("storedVideosUpdated.json", videosJson);

            //await mongoRepo.Save(storedVideosUpdated);
        }
    }
}
