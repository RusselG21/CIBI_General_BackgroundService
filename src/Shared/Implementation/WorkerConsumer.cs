namespace Shared.Implementation;

/// <summary>
/// Represents a worker that fetches data, processes it, and posts it to a specified URL.
/// </summary>
/// <typeparam name="TIn">The type of the incoming payload.</typeparam>
/// <typeparam name="TOut">The type of the transformed payload.</typeparam>
public class WorkerConsumer<TIn, TOut>(
    IServiceProvider serviceProvider,
    ILogger<WorkerConsumer<TIn, TOut>> logger,
    WorkerConsumerOptions options) : IWorkerConsumer<TIn, TOut>
{
    public async Task<int> HandleAsync(int page, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var fetcher = scope.ServiceProvider.GetRequiredService<IFetcher<TIn>>();
        var generateTokenBasicAuth = scope.ServiceProvider.GetRequiredService<IGenerateTokenBasicAuth>();

        logger.LogInformation("WorkerConsumer<{PayloadType}> started", typeof(TIn).Name);

        logger.LogInformation("WorkerConsumer<{PayloadType}>: Fetching from {Url} to get JWT Token", typeof(TIn).Name, options.FetchUrl);
        var bearerToken = await generateTokenBasicAuth.jWTTokenResponse(
            options.GenerateTokenUrl,
            options.Username,
            options.Password,
            cancellationToken);

        options.QueryParams["page"] = page.ToString();
        logger.LogInformation("WorkerConsumer<{PayloadType}>: Query Params are {QueryParams}", typeof(TIn).Name, JsonSerializer.Serialize(options.QueryParams));

        var fetchedPayloads = await fetcher.FetchAsync(options.FetchUrl, options.QueryParams, cancellationToken);

        if (fetchedPayloads is null || !fetchedPayloads.Any())
        {
            logger.LogWarning("WorkerConsumer<{PayloadType}>: No payloads to process", typeof(TIn).Name);
            return 1;
        }

        foreach (var talkPushPayload in fetchedPayloads)
        {
            try
            {
                using var scopedScope = serviceProvider.CreateScope();
                var scopedPoster = scopedScope.ServiceProvider.GetRequiredService<IPoster>();
                var scopedTransformer = scopedScope.ServiceProvider.GetRequiredService<ITransformer<TIn, TOut>>();

                var transformedPayload = scopedTransformer.Transform(talkPushPayload);

                logger.LogInformation(
                    "WorkerConsumer<{PayloadType}>: Posting payload to {Url}, Transformed Payload: {TransformedPayload}",
                    typeof(TIn).Name,
                    options.PostUrl,
                    JsonSerializer.Serialize(transformedPayload));

                var result = await scopedPoster.PostAsync(
                    options.PostUrl,
                    transformedPayload!,
                    talkPushPayload!,
                    bearerToken.Token,
                    cancellationToken);

                if (result == 200)
                {
                    logger.LogInformation("WorkerConsumer<{PayloadType}>: Successfully posted payload", typeof(TIn).Name);
                }
                else if (result == 401)
                {
                    logger.LogWarning("WorkerConsumer<{PayloadType}>: Unauthorized access", typeof(TIn).Name);
                }
                else if (result == 500)
                {
                    logger.LogWarning("WorkerConsumer<{PayloadType}>: Server error while posting payload", typeof(TIn).Name);
                }
                else
                {
                    logger.LogWarning("WorkerConsumer<{PayloadType}>: Unexpected response code {StatusCode}", typeof(TIn).Name, result);
                }

                // Optional cooldown between posts (tweak this based on testing)
                await Task.Delay(300, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WorkerConsumer<{PayloadType}>: Exception while posting payload for {User}", typeof(TIn).Name, JsonSerializer.Serialize(talkPushPayload));
            }
        }

        return page + 1;
    }
}
