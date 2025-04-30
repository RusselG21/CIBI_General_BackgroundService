namespace Talkpush_BackgroundService.Implementation;

public class Fetcher(
    HttpClient httpClient, 
    ILogger<IFetcher<Candidate>> logger) 
    : IFetcher<Candidate>
{
    public async Task<IEnumerable<Candidate>> FetchAsync(
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
        // Set default Accept header to application/json
        if (!httpClient.DefaultRequestHeaders.Accept.Any(h => h.MediaType == "application/json"))
        {
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        var httpResponse = await httpClient.GetAsync(enpoint);
        var contentType = httpResponse.Content.Headers.ContentType?.MediaType;

        if (contentType != "application/json")
        {
            var raw = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            logger.LogWarning("Received non-JSON content from {Url}: {Content}", url, raw);
            return default!;
        }

        // Check if response is successful
        if (!httpResponse.IsSuccessStatusCode)
        {
            logger.LogWarning("Received non-success status code {StatusCode} from {Url}",
                httpResponse.StatusCode, url);
            return default!;
        }

        // Deserialize the response content
        var payload = await httpResponse.Content.ReadFromJsonAsync<CandidateRoot>(cancellationToken: cancellationToken);

        if (payload == null)
        {
            logger.LogWarning("Received null payload from {Url}", url);
            return default!;
        }

        logger.LogInformation("Successfully fetched data from {Url}", url);
        return payload.Candidates;
    }
}
