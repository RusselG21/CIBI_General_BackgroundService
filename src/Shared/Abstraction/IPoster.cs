namespace Shared.Abstraction;

public interface IPoster
{
    Task<int> PostAsync(string url, object payload,object talkpushPayload, string bearerToken, CancellationToken cancellationToken);
}
