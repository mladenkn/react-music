using System.Linq;
using System.Threading.Tasks;
using Music.Repositories;
using Xunit;

namespace Executables
{
    public class YoutubeDataVideoRepositoryTest
    {
        [Fact]
        public async Task SearchItemsAreSameAsListItems()
        {
            var services = new Services();
            var query = new YoutubeTrackQuery
            {
                MaxResults = 10,
                SearchQuery = "i hate models",
            };

            var youtubeVideosFromSearch = await services.YoutubeDataApiVideoRepository.Search(query);
            var youtubeVideosFromSearchIds = youtubeVideosFromSearch.Select(v => v.Id).ToArray();

            var youtubeVideosFromList =
                await services.YoutubeDataApiVideoRepository.GetList(youtubeVideosFromSearchIds);

            foreach (var youtubeVideoFromSearch in youtubeVideosFromSearch)
            {
                var youtubeVideoFromList = youtubeVideosFromList.First(v => v.Id == youtubeVideoFromSearch.Id);

                Assert.Equal(youtubeVideoFromSearch.ChannelId, youtubeVideoFromList.ChannelId);
                Assert.Equal(youtubeVideoFromSearch.Description, youtubeVideoFromList.Description);
                Assert.Equal(youtubeVideoFromSearch.ChannelTitle, youtubeVideoFromList.ChannelTitle);
                Assert.Equal(youtubeVideoFromSearch.Duration, youtubeVideoFromList.Duration);
                Assert.Equal(youtubeVideoFromSearch.PublishedAt, youtubeVideoFromList.PublishedAt);
                Assert.Equal(youtubeVideoFromSearch.Title, youtubeVideoFromList.Title);
                Assert.Equal(youtubeVideoFromSearch.Thumbnails.Count(), youtubeVideoFromList.Thumbnails.Count());
            }
        }
    }
}
