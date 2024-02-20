using System.Reflection;
using DayTimeService.Daily;
using DayTimeService.Execute;
using GingerMintSoft.DayTime;
using GingerMintSoft.DayTime.Scheduler;
using Coordinate = GingerMintSoft.DayTime.Coordinate;
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
                _logger.LogInformation("DayTimeServiceWorker started at: {time}", DateTimeOffset.Now.ToLocalTime());
            }

            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var program = new Application().ReadWorkload($@"{currentPath}\DailyWorkload.json");

            var task = new RecurringTask(() =>
                {
                    var now = DateTime.Now.ToLocalTime();
                    var actDate = new DateTime(now.Year, now.Month, now.Day);

                    var day = new CalcDayTime().SunriseSunset(
                        actDate,
                        new Coordinate()
                        {
                            Latitude = program!.Program.Coordinate.Latitude,
                            Longitude = program.Program.Coordinate.Longitude
                        });

                    var execute = program.Program.Tasks[0];

                    _scheduler.AddTask(new DayTimeTask()
                    {
                        TaskId = execute.TaskId,
                        StartTime = execute.TaskId == "SunRise" ? day.SunRise : day.SunSet,
                        TaskAction = () =>
                        {
                            var bOk = Command.Execute(execute.Command);
                            _logger.LogInformation("DayTimeServiceWorker executing Task SunRise: {time}. Executed: {bool}", DateTimeOffset.Now.ToLocalTime(), bOk);

                            //turn on
                            _scheduler.RemoveTask("SunRise");
                        }
                    });

                    execute = program.Program.Tasks[1];

                    _scheduler.AddTask(new DayTimeTask()
                    {
                        TaskId = execute.TaskId,
                        StartTime = execute.TaskId == "SunRise" ? day.SunRise : day.SunSet,
                        TaskAction = () =>
                        {
                            var bOk = Command.Execute(execute.Command);
                            _logger.LogInformation("DayTimeServiceWorker executing Task SunSet: {time}. Executed: {bool}", DateTimeOffset.Now.ToLocalTime(), bOk);

                            //turn on
                            _scheduler.RemoveTask("SunRise");
                        }
                    });
                },
                DateTime.Today.AddDays(1).AddSeconds(5),
                //DateTime.Now.ToLocalTime().AddSeconds(5),
                TimeSpan.FromDays(1),
                "DayTimeServiceWorker"
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
