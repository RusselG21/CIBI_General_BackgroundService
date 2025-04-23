namespace Shared.ModelConfig;
public record WorkerConsumerOptions(
   Dictionary<string, string> QueryParams,
   string FetchUrl,
   string PostUrl,
   string GenerateTokenUrl,
   string Username,
   string Password
);
