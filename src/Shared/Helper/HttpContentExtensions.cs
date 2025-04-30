namespace Shared.Helper
{
    public static class HttpContentExtensions
    {
        public static async Task<T?> ReadAsAsync<T>(this HttpContent content)
        {
            var responseBody = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseBody);
        }
    }
}
