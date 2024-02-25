using System.Reflection;
using DayTimeService.Daily;
using DayTimeService.Execute;
using DayTimeService.Hardware;
using GingerMintSoft.DayTime.Scheduler;
using Task = System.Threading.Tasks.Task;
using DayTimeTask = GingerMintSoft.DayTime.Scheduler.Task;
using DayTimeTaskScheduler = GingerMintSoft.DayTime.Scheduler.TaskScheduler;

namespace DayTimeService
{
    public class DayTimeServiceWorker : BackgroundService
    {
        public enum Day { SunRise = 0, SunSet = 1, Undefined = 9999 }
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

            var execute = new Application().ReadWorkload(Platform.OperatingSystem == Platform.EnmOperatingSystem.Windows 
                ? $@"{currentPath}\DailyWorkload.json" 
                : $@"{currentPath}/DailyWorkload.json");

            // start importing program every midnight after 5 minutes
            var startingTime = DateTime.Today.AddDays(1).AddSeconds(5);
            var recurrence = execute!.Program.Recurrence ?? TimeSpan.FromDays(1);

            _logger.LogInformation(
                "DayTimeServiceWorker is executed at: {time} with recurrence {time}", 
                startingTime, 
                recurrence);

            var task = new RecurringTask(() => 
                {
                    //actual date and time set to midnight
                    var now = DateTime.Now.ToLocalTime();
                    var actDate = new DateTime(now.Year, now.Month, now.Day);

                    var day = Define.CalcSunRiseSunSet(actDate, execute);

                    foreach (var taskToExec in execute.Program.Tasks.OrderBy(tsk => tsk.Id))
                    {
                        _scheduler.AddTask(new DayTimeTask()
                        {
                            TaskId = taskToExec.TaskId,
                            StartTime = taskToExec.Id == Convert.ToInt32(Day.SunRise) 
                                ? day.SunRise 
                                : day.SunSet,
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
