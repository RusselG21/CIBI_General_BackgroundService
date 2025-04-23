namespace Shared.Implementation;

/// <summary>
/// Represents a worker that fetches data, processes it, and posts it to a specified URL.
/// </summary>
/// <typeparam name="T">The type of the payload being processed.</typeparam>
public class WorkerConsumer<T>(
     IFetcher<T> fetcher,
     IPoster poster,
     IGenerateTokenBasicAuth generateTokenBasicAuth,
     ILogger<WorkerConsumer<T>> logger,
     WorkerConsumerOptions options) : IWorkerConsumer<T>
{
    private static readonly AsyncRetryPolicy RetryPolicy = Policy
       .Handle<Exception>()
       .WaitAndRetryAsync(3, retryAttempt =>
           TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    private const int MaxConcurrency = 3;
    private readonly SemaphoreSlim _semaphore = new(MaxConcurrency);

    public async Task HandleAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("WorkerConsumer<{PayloadType}> started", typeof(T).Name);

        // logger for consuming specific url
        logger.LogInformation("WorkerConsumer<{PayloadType}>: Fetching from {Url} to get JWT Token", typeof(T).Name, options.FetchUrl);
        var bearerToken = await generateTokenBasicAuth.jWTTokenResponse(
            options.GenerateTokenUrl,
            options.Username,
            options.Password,
            cancellationToken);


        var fetchedPayloads = await fetcher.FetchAsync(options.FetchUrl, options.QueryParams, cancellationToken);

        if (fetchedPayloads is null)
        {
            logger.LogWarning("WorkerConsumer<{PayloadType}>: No payloads found", typeof(T).Name);
            return;
        }

        var tasks = fetchedPayloads.Select(async payload =>
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                await RetryPolicy.ExecuteAsync(async () =>
                {
                    var result = await poster.PostAsync(
                        options.PostUrl,
                        payload!,
                        bearerToken.Token,
                        cancellationToken);
                    // Unauthorized access
                    if (result == 401)
                    {
                        logger.LogWarning("WorkerConsumer<{PayloadType}>: Unauthorized access", typeof(T).Name);
                    }
                    // Server error
                    if (result == 500)
                    {
                        logger.LogWarning("WorkerConsumer<{PayloadType}>: Failed to post payload", typeof(T).Name);
                    }
                    // Success
                    if (result == 200)
                    {
                        logger.LogInformation("WorkerConsumer<{PayloadType}>: Successfully posted payload", typeof(T).Name);
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WorkerConsumer<{PayloadType}>: Exception while posting payload", typeof(T).Name);
            }
            finally
            {
                _semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }
}
