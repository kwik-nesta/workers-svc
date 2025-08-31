using System.Text.Json;

namespace KwikNesta.Workers.Svc.Application.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T?> ReadFromJsonAsync<T>(this HttpResponseMessage response,
                                                          JsonSerializerOptions? options = null)
        {
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(stream, options ?? new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
