using System.Text.Json;

namespace Shared.Implementation;

public class Poster(
     HttpClient httpClient, 
     ILogger<Poster> logger) : IPoster
{
    public async Task<int> PostAsync(
        string url, 
        object payload, 
        object talkPushPayload,
        string bearerToken, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Posting payload to {Url}", url);

        // Validate parameters
        if (payload == null || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(bearerToken))
        {
            logger.LogWarning("Invalid parameters: url: {Url}, payload: {Payload}, bearerToken: {BearerToken}", url, JsonSerializer.Serialize(payload), bearerToken);
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

        var responseBody = await response.Content.ReadAsStringAsync();

        // logger for response body
        logger.LogWarning("Response from {Url}, Payload Request {PayloadRequest} , Response Body {ResponseBody}", url, JsonSerializer.Serialize(payload), responseBody);

        // Log the response if not successful
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Failed to POST payload. Status: {StatusCode}", response.StatusCode);
            return 400;
        }

        // Log the successful response
        logger.LogInformation("Payload successfully posted to {Endpoint}", url);
        return 200;

    }
}
