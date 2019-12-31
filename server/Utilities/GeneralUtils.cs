using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Utilities
{
    public static class GeneralUtils
    {
        public static Task<HttpResponseMessage> PostJsonAsync(this HttpClient httpClient, string url, object content)
        {
            var content_ = new StringContent(
                JsonConvert.SerializeObject(content),
                Encoding.UTF8,
                "application/json"
            );
            return httpClient.PostAsync(url, content_);
        }

        public static bool IsAnyOf(this object o, params object[] values) => values.Contains(o);
    }
}
