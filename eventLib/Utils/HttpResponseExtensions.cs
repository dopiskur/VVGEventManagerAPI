using System.Net.Http.Json;

namespace eventLib.Utils
{
    public static class HttpResponseExtensions
    {
        public static async Task<T?> ReadContentAsync<T>(this HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
