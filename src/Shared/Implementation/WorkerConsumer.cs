namespace Shared.Implementation;

/// <summary>
/// Represents a worker that fetches data, processes it, and posts it to a specified URL.
/// </summary>
/// <typeparam name="T">The type of the payload being processed.</typeparam>
/// <param name="queryParams">A dictionary of query parameters to be used when fetching data.</param>
/// <param name="fetcher">An implementation of <see cref="IFetcher{T}"/> used to fetch data from a specified URL.</param>
/// <param name="poster">An implementation of <see cref="IPoster{T}"/> used to post data to a specified URL.</param>
/// <param name="logger">An instance of <see cref="ILogger{WorkerConsumer{T}}"/> used for logging information, warnings, and errors.</param>
/// <param name="postURL">The URL to which the payload will be posted.</param>
/// <param name="bearerToken">The bearer token used for authentication when posting the payload.<
public class WorkerConsumer<T>(
     Dictionary<string, string> queryParams,
     IFetcher<T> fetcher,
     IPoster<T> poster,
     ILogger<WorkerConsumer<T>> logger,
     string postURL,
     string bearerToken) : IWorkerConsumer<T>
{
    // for retrying failed requests
    private static readonly AsyncRetryPolicy RetryPolicy = Policy
       .Handle<Exception>()
       .WaitAndRetryAsync(3, retryAttempt =>
           TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));


    public async Task HandleAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("WorkerConsumer<{PayloadType}> started", typeof(T).Name);

        // fetch the payload from the specified URL
        var fetchedPayload = await fetcher.FetchAsync(postURL, queryParams, cancellationToken);

        // if payload is null, log a warning and return
        if (fetchedPayload is null)
        {
            logger.LogWarning("WorkerConsumer<{PayloadType}>: Payload is null", typeof(T).Name);
            return;
        }

        await RetryPolicy.ExecuteAsync(async () =>
        {
            // post the payload to the specified URL
            var success = await poster.PostAsync(
                postURL,
                fetchedPayload, 
                bearerToken, 
                cancellationToken);

            // if the post was not successful, log a warning
            if (!success)
            {
                logger.LogWarning("WorkerConsumer<{PayloadType}>: Failed to post payload", typeof(T).Name);
            }

            // if the post was successful, log an information message
            logger.LogInformation("WorkerConsumer<{PayloadType}>: Successfully posted payload", typeof(T).Name);
        });

    }
}
