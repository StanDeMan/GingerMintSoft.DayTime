using DayTimeService.Services;

namespace DayTimeService
{
    public class Program
    {
        public static async Task Main(string?[] args)
        {
            var host = Host.CreateDefaultBuilder(args!)
                .UseSystemd()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton(new ArgumentService(args));
                    services.AddHostedService<DayTimeServiceWorker>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}