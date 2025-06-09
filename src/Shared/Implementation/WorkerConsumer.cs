using System.Text.Json;
using Shared.Abstraction;

namespace Shared.Implementation;

/// <summary>
/// Represents a worker that fetches data, processes it, and posts it to a specified URL.
/// </summary>
/// <typeparam name="T">The type of the payload being processed.</typeparam>
public class WorkerConsumer<TIn,TOut>(
     IServiceProvider serviceProvider,
     ILogger<WorkerConsumer<TIn, TOut>> logger,
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
        using var scope = serviceProvider.CreateScope();

        var fetcher = scope.ServiceProvider.GetRequiredService<IFetcher<TIn>>();
        var generateTokenBasicAuth = scope.ServiceProvider.GetRequiredService<IGenerateTokenBasicAuth>();

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
                // Setting up scope service
                using var scope = serviceProvider.CreateScope();
                var scopedPoster = scope.ServiceProvider.GetRequiredService<IPoster>();
                var scopedTransformer = scope.ServiceProvider.GetRequiredService<ITransformer<TIn, TOut>>();
                var transformedPayload = scopedTransformer.Transform(talkPushPayload);

                logger.LogInformation("WorkerConsumer<{PayloadType}>: Posting payload to {Url} , Transformed Payload is {TransformedPayload}", typeof(TIn).Name, options.PostUrl, JsonSerializer.Serialize(transformedPayload));

                await RetryPolicy.ExecuteAsync(async () =>
                {
                    var result = await scopedPoster.PostAsync(
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
                logger.LogError(ex, "WorkerConsumer<{PayloadType}>: Exception while posting payload for {user}", typeof(TIn).Name, JsonSerializer.Serialize(talkPushPayload));
            }
            finally
            {
                _semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }
}
