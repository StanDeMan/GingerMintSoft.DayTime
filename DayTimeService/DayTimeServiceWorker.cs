using System.Reflection;
using DayTimeService.Daily;
using DayTimeService.Daily.Jobs;
using DayTimeService.Execute;
using DayTimeService.Hardware;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Task = System.Threading.Tasks.Task;

namespace DayTimeService
{
    public class DayTimeServiceWorker(ILogger<DayTimeServiceWorker> logger) : BackgroundService
    {
        public enum Day { Undefined = 9999, SunRise = 0, SunSet = 1,  }
        private bool _ledOn;
        private readonly ILogger<DayTimeServiceWorker> _logger = logger;

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

            await StartSunRiseSunSetScheduler(stoppingToken, execute, recurrence, startingTime);

            while (!stoppingToken.IsCancellationRequested)
            {
                // do some blinking here
                Command.Execute((_ledOn ? "w 14 1" : "w 14 0"));
                _ledOn = !_ledOn;

                await Task.Delay(250, stoppingToken);
            }
        }

        /// <summary>
        /// Start all over scheduler for sun rise and sun set calculation and command execution
        /// </summary>
        /// <param name="stoppingToken">Thread stopping</param>
        /// <param name="execute">All parameters for execution</param>
        /// <param name="recurrence">In which interval should the execution take place</param>
        /// <param name="startingTime">The first trigger time</param>
        /// <returns></returns>
        private static async Task StartSunRiseSunSetScheduler(CancellationToken stoppingToken,
            Workload execute,
            TimeSpan recurrence,
            DateTime startingTime)
        {
            var scheduler = await new StdSchedulerFactory().GetScheduler(stoppingToken);
            await scheduler.Start(stoppingToken);

            var job = JobBuilder.Create<DailyJob>()
                .WithIdentity(execute.Program.TaskId)
                .UsingJobData("Execute", JsonConvert.SerializeObject(execute))
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(s =>
                    s.WithIntervalInHours(Convert.ToInt32(recurrence.TotalHours))
                        .OnEveryDay()
                        //.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(
                        //        startingTime.Hour,
                        //        startingTime.Minute)
                        .StartingDailyAt(new TimeOfDay(DateTime.Now.Hour, DateTime.Now.Minute,DateTime.Now.Second)
                        )
                ).Build();

            await scheduler.ScheduleJob(job, trigger, stoppingToken);
        }
    }
}
