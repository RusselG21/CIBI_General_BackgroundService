namespace Shared.Abstraction;

public interface IPoster
{
    Task<int> PostAsync(string url, object payload, string bearerToken, CancellationToken cancellationToken);
}
