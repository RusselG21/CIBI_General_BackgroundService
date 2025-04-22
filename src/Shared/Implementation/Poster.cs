namespace Shared.Implementation;

public class Poster(
     HttpClient httpClient, 
     ILogger<Poster> logger) : IPoster
{
    public async Task<int> PostAsync(
        string url, 
        object payload, 
        string bearerToken, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Posting payload to {Url}", url);

        // Validate parameters
        if (payload == null || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(bearerToken))
        {
            logger.LogWarning("Invalid parameters: url: {Url}, payload: {Payload}, bearerToken: {BearerToken}", url, payload, bearerToken);
            return 500;
        }

        // Create the request message
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(payload)
        };

        // Set the authorization header
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        // Send the request
        var response = await httpClient.SendAsync(request, cancellationToken);

        // Log the response if not successful
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Failed to POST payload. Status: {StatusCode}", response.StatusCode);
            return (int)response.StatusCode;
        }

        // Log the successful response
        logger.LogInformation("Payload successfully posted to {Endpoint}", url);
        return (int)response.StatusCode;

    }
}
