using GingerMintSoft.DayTime;
using GingerMintSoft.DayTime.Scheduler;
using Task = System.Threading.Tasks.Task;
using DayTimeTask = GingerMintSoft.DayTime.Scheduler.Task;
using DayTimeTaskScheduler = GingerMintSoft.DayTime.Scheduler.TaskScheduler;

namespace DayTimeService
{
    public class DayTimeServiceWorker : BackgroundService
    {
        private readonly DayTimeTaskScheduler _scheduler = new();

        private readonly ILogger<DayTimeServiceWorker> _logger;

        public DayTimeServiceWorker(ILogger<DayTimeServiceWorker> logger)
        {
            _logger = logger;
            _scheduler.Start();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("GingerMintSoftware.DayTimeServiceWorker started at: {time}", DateTimeOffset.Now.ToLocalTime());
            }

            var task = new RecurringTask(() =>
                {
                    var now = DateTime.Now.ToLocalTime();
                    var actDate = new DateTime(now.Year, now.Month, now.Day);

                    var day = new CalcDayTime().SunriseSunset(
                        actDate,
                        new Coordinate()
                        {
                            Latitude = 48.10507778308992,
                            Longitude = 7.90856839921184
                        });

                    _scheduler.AddTask(new DayTimeTask()
                    {
                        TaskId = "SunRise",
                        StartTime = day.SunRise,
                        TaskAction = () =>
                        {
                            _logger.LogInformation("GingerMintSoftware.DayTimeServiceWorker executing Task SunRise: {time}", DateTimeOffset.Now.ToLocalTime());

                            //turn on
                            _scheduler.RemoveTask("SunRise");
                        }
                    });

                    _scheduler.AddTask(new DayTimeTask()
                    {
                        TaskId = "SunSet",
                        StartTime = day.SunSet,
                        TaskAction = () =>
                        {
                            _logger.LogInformation("GingerMintSoftware.DayTimeServiceWorker executing Task SunSet: {time}", DateTimeOffset.Now.ToLocalTime());

                            //turn off
                            _scheduler.RemoveTask("SunSet");
                        }
                    });
                },
                DateTime.Today.AddDays(1).AddSeconds(5),
                //DateTime.Now.ToLocalTime().AddSeconds(5),
                TimeSpan.FromDays(1),
                "DayTimeService"
            );

            _scheduler.AddTask(task);

            while (!stoppingToken.IsCancellationRequested)
            {
                // do some blinking here
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
