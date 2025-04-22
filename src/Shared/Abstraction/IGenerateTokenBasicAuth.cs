namespace Shared.Abstraction;

public interface IGenerateTokenBasicAuth
{
    Task<JWTTokenResponse> jWTTokenResponse(
        string url,
        string username,
        string password,
        CancellationToken cancellationToken = default);
}
