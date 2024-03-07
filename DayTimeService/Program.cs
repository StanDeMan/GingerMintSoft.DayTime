using CommandLine;
using DayTimeService.CmdLine;
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
                    services.AddSingleton(
                        new ArgumentService(
                            new Parser(with => with.EnableDashDash = true)
                                .ParseArguments<Options>(args)));

                    services.AddHostedService<DayTimeServiceWorker>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}