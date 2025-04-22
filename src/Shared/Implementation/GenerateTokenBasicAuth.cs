namespace Shared.Implementation
{
    public class GenerateTokenBasicAuth : IGenerateTokenBasicAuth
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GenerateTokenBasicAuth> _logger;

        // Constructor for Dependency Injection
        public GenerateTokenBasicAuth(HttpClient httpClient, ILogger<GenerateTokenBasicAuth> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<JWTTokenResponse> jWTTokenResponse(
            string url,
            string username,
            string password,
            CancellationToken cancellationToken)
        {
            // Set the base address of the HttpClient if needed
            _logger.LogInformation("Setting base address for HttpClient");
            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            var base64Auth = Convert.ToBase64String(byteArray);

            // Set the Authorization header
            _logger.LogInformation("Setting Authorization header for HttpClient");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Send the request
            _logger.LogInformation("Sending request to {Url}", url);
            HttpResponseMessage response = null!;
            try
            {
                response = await _httpClient.PostAsync(url, null, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending request to {Url}", url);
                return new JWTTokenResponse(string.Empty, string.Empty, string.Empty); // Return empty on failure
            }

            // Log the response
            _logger.LogInformation("Received response from {Url}", url);
            var token = await response.Content.ReadFromJsonAsync<JWTTokenResponse>(cancellationToken);

            // Check if the response is not successful
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Received non-success status code {StatusCode} from {Url}", response.StatusCode, url);
                return token ?? new JWTTokenResponse(string.Empty, string.Empty, string.Empty); // Return empty if token is null
            }

            _logger.LogInformation("Successfully received token from {Url}", url);
            return token!;
        }
    }
}
