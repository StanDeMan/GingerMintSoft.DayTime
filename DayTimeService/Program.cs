namespace DayTimeService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<DayTimeServiceWorker>();
            builder.Logging.AddConsole();

            var host = builder.Build();
            host.Run();
        }
    }
}