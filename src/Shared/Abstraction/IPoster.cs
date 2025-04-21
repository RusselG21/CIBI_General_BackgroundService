namespace Shared.Abstraction;

public interface IPoster<TPayload>
{
    Task<bool> PostAsync(string url, TPayload payload, string bearerToken, CancellationToken cancellationToken);
}
