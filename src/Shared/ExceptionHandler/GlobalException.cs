namespace Shared.ExceptionHandler;
public class GlobalException : IExceptionHandler
{
    private readonly ILogger<GlobalException> _logger;

    public GlobalException(ILogger<GlobalException> logger)
    {
        this._logger = logger;
    }

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var exceptionMessage = exception.Message;

        _logger.LogError(
            "Error Message: {exceptionMessage}",
            exceptionMessage);

        return ValueTask.FromResult(false);
    }
}
