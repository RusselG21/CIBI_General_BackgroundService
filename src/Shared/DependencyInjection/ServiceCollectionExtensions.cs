using Microsoft.Extensions.Configuration;

namespace Shared.DependencyInjection;

public static class ServiceCollectionExtensions
{
    #region Services 
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPoster), typeof(Poster));
        services.AddScoped(typeof(IWorkerConsumer<>), typeof(WorkerConsumer<>));
        services.AddScoped(typeof(IGenerateTokenBasicAuth), typeof(GenerateTokenBasicAuth));
        return services;
    }
    #endregion

    #region Logger
    public static void ConfigureLogger(this IServiceCollection services, IConfiguration configuration)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var logFilePath = configuration["Logging:LogFilePath"];
        var fullLogFilePath = Path.Combine(currentDirectory, logFilePath!);

        // Ensure the directory exists
        var logDirectory = Path.GetDirectoryName(fullLogFilePath);
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory!);
        }
        var logger = new LoggerConfiguration()
          .WriteTo.Console()
          .WriteTo.File(fullLogFilePath, rollingInterval: RollingInterval.Day)
          .MinimumLevel
          .Information()
          .CreateLogger();
        Log.Logger = logger;
        services.AddSingleton<Serilog.ILogger>(logger);

    }
    #endregion

}
