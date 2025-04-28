namespace Shared.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle
        (TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("[START] Handling request={Request} - Response={Response} - RequestData={RequesData}",
            typeof(TRequest).Name, typeof(TResponse).Name, request);

        var timer = new Stopwatch();
        timer.Start();

        var response = await next();

        timer.Stop();
        var timeTaken = timer.Elapsed;
        if (timeTaken.Seconds > 3) // if the request takes more than 3 seconds, log it as a warning
        {
            logger.LogWarning("[END] Handling request={Request} - Response={Response} - TimeTaken={TimeTaken}",
                typeof(TRequest).Name, typeof(TResponse).Name, timeTaken);

            return response;
        }

        logger.LogInformation("[END] Handling request={Request} - Response={Response} - TimeTaken={TimeTaken}",
            typeof(TRequest).Name, typeof(TResponse).Name, timeTaken);

        return response;
    }
}

