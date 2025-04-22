namespace Shared.Abstraction;

public interface IFetcher<TReturn>
{
    Task<IEnumerable<TReturn>> FetchAsync(string url, Dictionary<string, string> queryParams, CancellationToken cancellationToken);
}
