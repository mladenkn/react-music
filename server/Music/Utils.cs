using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Music
{
    public static class Utils
    {
        public static async Task<List<PlaylistItem>> ExecuteWithPagingAsync(this PlaylistItemsResource.ListRequest request)
        {
            var result = new List<PlaylistItem>();
            var remainingCount = request.MaxResults;
            string nextPageToken = null;
            do
            {
                request.MaxResults = remainingCount;
                request.PageToken = nextPageToken;
                var response = await request.ExecuteAsync();
                result.AddRange(response.Items);
                nextPageToken = response.NextPageToken;
                remainingCount -= response.Items.Count;
            }
            while (nextPageToken != null && remainingCount != 0);

            return result;
        } 
    }
}
