using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MongoDB.Driver;
using Music.Repositories;
using Newtonsoft.Json;
using Xunit;

namespace Operations
{
    public class UpdateYoutubeVideosInMongoDb
    {
        [Fact]
        public async Task Run()
        {
            var youtubeServiceParams = new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyA1xQd0rfJCzG1ghK7RoKRI7EfakGLfDZM"
            };
            var remoteRepo = new YoutubeDataApiVideoRepository(new YouTubeService(youtubeServiceParams));
            
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var mongoDatabase = mongoClient.GetDatabase("music");
            var mongoRepo = new YoutubeVideoMongoRepository(mongoDatabase);

            var storedVideosIds = await mongoRepo.GetAllIds();
            var storedVideosUpdated = await remoteRepo.GetList(storedVideosIds.ToArray());

            var filePath = @"C:\Users\a\Documents\projekti\music\server\Operations\Results\storedVideosUpdated.json";
            var videosJson = JsonConvert.SerializeObject(storedVideosUpdated, Formatting.Indented);
            File.WriteAllText(filePath, videosJson);

            //await mongoRepo.Save(storedVideosUpdated);
        }
    }
}
