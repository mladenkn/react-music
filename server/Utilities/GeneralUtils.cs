using System.Collections.Generic;
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

        public static async Task<T> ParseAsJson<T>(this HttpContent httpContent)
        {
            var contentString = await httpContent.ReadAsStringAsync();
            var parsed = JsonConvert.DeserializeObject<T>(contentString);
            return parsed;
        }

        public static bool IsAnyOf(this object o, params object[] values) => values.Contains(o);

        public static bool IsIn<T>(this T o, IEnumerable<T> enumerable) => enumerable.Contains(o);

        public static T Get<T>(this IReadOnlyDictionary<string, object> dict, string key) => (T) dict[key];

        public static T GetOrDefault<T>(this IReadOnlyDictionary<string, object> dict, string key)
        {
            var valueOrDefault = dict.GetValueOrDefault(key);
            return valueOrDefault == default ? default(T) : (T) valueOrDefault;
        }
    }
}
