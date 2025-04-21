namespace Shared.Abstraction;

public interface IFetcher<TReturn>
{
    Task<TReturn> FetchAsync(string url, Dictionary<string, string> queryParams, CancellationToken cancellationToken);
}
