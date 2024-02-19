namespace DayTimeService
{
    public class DayTimeServiceWorker : BackgroundService
    {
        private readonly ILogger<DayTimeServiceWorker> _logger;

        public DayTimeServiceWorker(ILogger<DayTimeServiceWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("GingerMintSoftware.DayTimeService Worker running at: {time}", DateTimeOffset.Now);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
