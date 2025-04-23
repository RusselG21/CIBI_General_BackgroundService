using Microsoft.Extensions.Hosting;

namespace Shared.Implementation
{
    public class BackgroundWorker<T> : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BackgroundWorker<T>> _logger;

        public BackgroundWorker(IServiceProvider serviceProvider, ILogger<BackgroundWorker<T>> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
         
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BackgroundWorker<{PayloadType}> is starting.", typeof(T).Name);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var consumer = scope.ServiceProvider.GetRequiredService<IWorkerConsumer<T>>();
                    await consumer.HandleAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing item in BackgroundWorker<{PayloadType}>", typeof(T).Name);
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken); // wait before next trigger
            }
        }
    }
}

