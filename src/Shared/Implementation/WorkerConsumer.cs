using System.Text.Json;
using Shared.Abstraction;

namespace Shared.Implementation;

/// <summary>
/// Represents a worker that fetches data, processes it, and posts it to a specified URL.
/// </summary>
/// <typeparam name="T">The type of the payload being processed.</typeparam>
public class WorkerConsumer<TIn,TOut>(
     IFetcher<TIn> fetcher,
     IPoster poster,
     IGenerateTokenBasicAuth generateTokenBasicAuth,
     ILogger<WorkerConsumer<TIn, TOut>> logger,
     ITransformer<TIn, TOut> transformer,
     WorkerConsumerOptions options) : IWorkerConsumer<TIn, TOut>
{
    private static readonly AsyncRetryPolicy RetryPolicy = Policy
       .Handle<Exception>()
       .WaitAndRetryAsync(10, retryAttempt =>
           TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    private const int MaxConcurrency = 10;
    private readonly SemaphoreSlim _semaphore = new(MaxConcurrency);

    public async Task HandleAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("WorkerConsumer<{PayloadType}> started", typeof(TIn).Name);

        // logger for consuming specific url
        logger.LogInformation("WorkerConsumer<{PayloadType}>: Fetching from {Url} to get JWT Token", typeof(TIn).Name, options.FetchUrl);
        var bearerToken = await generateTokenBasicAuth.jWTTokenResponse(
            options.GenerateTokenUrl,
            options.Username,
            options.Password,
            cancellationToken);


        var fetchedPayloads = await fetcher.FetchAsync(options.FetchUrl, options.QueryParams, cancellationToken);

        if (fetchedPayloads is null)
        {
            logger.LogWarning("WorkerConsumer<{PayloadType}>: No payloads found", typeof(TIn).Name);
            return;
        }

        var tasks = fetchedPayloads.Select(async talkPushPayload =>
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                var transformedPayload = transformer.Transform(talkPushPayload);

                logger.LogInformation("WorkerConsumer<{PayloadType}>: Posting payload to {Url} , Transformed Payload is {TransformedPayload}", typeof(TIn).Name, options.PostUrl, JsonSerializer.Serialize(transformedPayload));

                await RetryPolicy.ExecuteAsync(async () =>
                {
                    var result = await poster.PostAsync(
                        options.PostUrl,
                        transformedPayload!,
                        talkPushPayload!,
                        bearerToken.Token,
                        cancellationToken);

                    // Unauthorized access
                    if (result == 401)
                    {
                        logger.LogWarning("WorkerConsumer<{PayloadType}>: Unauthorized access", typeof(TIn).Name);
                    }
                    // Server error
                    if (result == 500)
                    {
                        logger.LogWarning("WorkerConsumer<{PayloadType}>: Failed to post payload", typeof(TIn).Name);
                    }
                    // Success
                    if (result == 200)
                    {
                        logger.LogInformation("WorkerConsumer<{PayloadType}>: Successfully posted payload", typeof(TIn).Name);
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WorkerConsumer<{PayloadType}>: Exception while posting payload", typeof(TIn).Name);
            }
            finally
            {
                _semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }
}
