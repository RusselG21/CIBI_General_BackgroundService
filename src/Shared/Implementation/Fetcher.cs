namespace Shared.Implementation;

public class Fetcher<TReturn>(HttpClient httpClient, ILogger<IFetcher<TReturn>> logger) : IFetcher<TReturn>
{
    public async Task<TReturn> FetchAsync(
        string url, 
        Dictionary<string,string> queryParams, 
        CancellationToken cancellationToken)
    {
        var enpoint = "";

        if (queryParams != null && queryParams.Count > 0)
        {
            enpoint = QueryHelpers.AddQueryString(url, queryParams!);    
        }

        logger.LogInformation("Fetching data from {Url}", enpoint);
        var httpResponse = await httpClient.GetAsync(enpoint);

        // Check if response is successful
        if (!httpResponse.IsSuccessStatusCode)
        {
            logger.LogWarning("Received non-success status code {StatusCode} from {Url}",
                httpResponse.StatusCode, url);
            return default;
        }

        // Deserialize the response content
        var payload = await httpResponse.Content.ReadFromJsonAsync<TReturn>(cancellationToken: cancellationToken);

        if (payload == null)
        {
            logger.LogWarning("Received null payload from {Url}", url);
            return default;
        }

        logger.LogInformation("Successfully fetched data from {Url}", url);
        return payload;
    }
}
