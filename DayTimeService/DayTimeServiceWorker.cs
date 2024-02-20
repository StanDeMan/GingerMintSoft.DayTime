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
        private bool _ledOn;
        private readonly DayTimeTaskScheduler _scheduler = new();
        private readonly ILogger<DayTimeServiceWorker> _logger;

        public DayTimeServiceWorker(ILogger<DayTimeServiceWorker> logger)
        {
            _logger = logger;
            _scheduler.Start();
        }

        /// <summary>
        /// Long term service
        /// </summary>
        /// <param name="stoppingToken">Stopping long term service</param>
        /// <returns>Exit code</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "DayTimeServiceWorker started at: {time}", 
                DateTimeOffset.Now.ToLocalTime());

            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var program = new Application().ReadWorkload($@"{currentPath}\DailyWorkload.json");

            var startingTime = DateTime.Today.AddDays(1).AddSeconds(5);
            var recurrence = TimeSpan.FromDays(1);

            _logger.LogInformation(
                "DayTimeServiceWorker is executed at: {time} with recurrence {time}", 
                startingTime, 
                recurrence);

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

                    foreach (var taskToExec in program.Program.Tasks)
                    {
                        taskToExec.ExecutionDateTime = taskToExec.TaskId == "SunRise" ? day.SunRise : day.SunSet;

                        _scheduler.AddTask(new DayTimeTask()
                        {
                            TaskId = taskToExec.TaskId,
                            StartTime = taskToExec.ExecutionDateTime,
                            TaskAction = () =>
                            {
                                var bOk = Command.Execute(taskToExec.Command);

                                _logger.LogInformation(
                                    "DayTimeServiceWorker executing Task {string} with command: {string} at {time}. Executed: {bool}", 
                                    taskToExec.Command,
                                    taskToExec.TaskId, 
                                    DateTimeOffset.Now.ToLocalTime(), 
                                    bOk);

                                _scheduler.RemoveTask($"{taskToExec.TaskId}");
                            }
                        });
                    }
                },
                startingTime,
                recurrence,
                "DayTimeServiceWorker");

            _scheduler.AddTask(task);

            while (!stoppingToken.IsCancellationRequested)
            {
                // do some blinking here
                Command.Execute((_ledOn ? "w 4 1" : "w 4 0"));
                _ledOn = !_ledOn;

                await Task.Delay(250, stoppingToken);
            }
        }
    }
}
