using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MongoDB.Driver;
using Music.Repositories;

namespace Executables
{
    public class Services
    {
        public Services()
        {
            var youtubeServiceParams = new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyA1xQd0rfJCzG1ghK7RoKRI7EfakGLfDZM"
            };
            YoutubeDataApiVideoRepository = new YoutubeDataApiVideoRepository(new YouTubeService(youtubeServiceParams));

            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var mongoDatabase = mongoClient.GetDatabase("music");
            YoutubeVideoMongoRepository = new YoutubeVideoMongoRepository(mongoDatabase);
        }

        public YoutubeDataApiVideoRepository YoutubeDataApiVideoRepository { get; set; }

        public YoutubeVideoMongoRepository YoutubeVideoMongoRepository { get; set; }
    }
}
