using Microsoft.Extensions.Hosting;

namespace Shared.Implementation
{
    public class BackgroundWorker<TIn, TOut> : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BackgroundWorker<TIn, TOut>> _logger;

        public BackgroundWorker(IServiceProvider serviceProvider, ILogger<BackgroundWorker<TIn, TOut>> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
         
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BackgroundWorker<{PayloadType}> is starting.", typeof(TIn).Name);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var consumer = scope.ServiceProvider.GetRequiredService<IWorkerConsumer<TIn, TOut>>();
                    await consumer.HandleAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing item in BackgroundWorker<{PayloadType}>", typeof(TIn).Name);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // wait before next trigger
            }
        }
    }
}

