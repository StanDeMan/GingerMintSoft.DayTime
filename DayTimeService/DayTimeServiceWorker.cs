using System.Reflection;
using DayTimeService.Daily;
using DayTimeService.Daily.Jobs;
using DayTimeService.Execute;
using DayTimeService.Hardware;
using DayTimeService.Services;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Task = System.Threading.Tasks.Task;

namespace DayTimeService
{
    public class DayTimeServiceWorker(
        ILogger<DayTimeServiceWorker> logger, 
        ArgumentService arguments) : BackgroundService
    {
        public enum EnmDay
        {
            // ReSharper disable once UnusedMember.Global
            Undefined = -1, 
            SunRise = 0, 
            SunSet = 1,
        }

        private bool _ledOn;

        /// <summary>
        /// Long term service:
        /// Switch pv accumulator storage by sun rise and sun set 
        /// times calculated by geo coordinates read from a json
        /// </summary>
        /// <param name="stoppingToken">Stopping long term service</param>
        /// <returns>Exit code</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var error = false;
            const int blinkError = 100;
            const int blinkNormal = 250;

            try
            {
                await DayTimeScheduler(stoppingToken);
            }
            catch (Exception e)
            {
                error = true;
                logger.LogError("Error at DayTimeServiceWorker.ExecuteAsync: {string}", e);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                // do some blinking here
                Command.Execute((_ledOn ? "w 14 1" : "w 14 0"));
                _ledOn = !_ledOn;

                await Task.Delay(error 
                    ? blinkError 
                    : blinkNormal, 
                    stoppingToken);
            }
        }

        /// <summary>
        /// Switch pv accumulator storage by sun rise and
        /// sun set times calculated by geo coordinates
        /// </summary>
        /// <param name="stoppingToken">Let the service know if it should stop</param>
        /// <returns>Exit code</returns>
        private async Task DayTimeScheduler(CancellationToken stoppingToken)
        {
            logger.LogInformation(
                "DayTimeServiceWorker started at: {time}",
                DateTimeOffset.Now.ToLocalTime());

            var workloadFile = arguments.Read().WorkloadFile ?? "DailyWorkload.json";
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var execute = new Application().ReadWorkload(Platform.OperatingSystem == Platform.EnmOperatingSystem.Windows
                ? $@"{currentPath}\{workloadFile}"
                : $"{currentPath}/{workloadFile}");

            // start calculate sun rise/set every midnight after 5 seconds
            var startingTime = DateTime.Today.AddDays(1).AddSeconds(5);
            var recurrence = execute!.Program.Recurrence ?? TimeSpan.FromDays(1);

            logger.LogInformation(
                "DayTimeServiceWorker will be executed at: {time} with recurrence of {double:F} hours",
                startingTime,
                recurrence.TotalHours);

            await StartDayTimeScheduler(stoppingToken, execute, recurrence, startingTime);
        }

        /// <summary>
        /// Start all over scheduler for sun rise and sun set calculation and command execution
        /// </summary>
        /// <param name="stoppingToken">Thread stopping</param>
        /// <param name="execute">All parameters for execution</param>
        /// <param name="recurrence">In which interval should the execution take place</param>
        /// <param name="startingTime">The first trigger time</param>
        /// <returns></returns>
        private static async Task StartDayTimeScheduler(CancellationToken stoppingToken,
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

            var trigger = BuildTrigger(execute, recurrence, startingTime);
            await scheduler.ScheduleJob(job, trigger, stoppingToken);
        }

        /// <summary>
        /// Build test or normal triggers
        /// </summary>
        /// <param name="execute">Parameter for execution</param>
        /// <param name="recurrence">Interval for recurrence</param>
        /// <param name="startingTime">Start execution</param>
        /// <returns>Configured trigger</returns>
        private static ITrigger BuildTrigger(
            Workload execute, 
            TimeSpan recurrence,
            DateTime startingTime)
        {
            return execute.Program.Test is { Active: true }
                ? BuildTestTrigger(execute) 
                : BuildDayTimeServiceTrigger(recurrence, startingTime);
        }

        /// <summary>
        /// Build normal trigger
        /// </summary>
        /// <param name="recurrence">Interval for recurrence</param>
        /// <param name="startingTime">Start execution at this time</param>
        /// <returns>Normal trigger</returns>
        private static ITrigger BuildDayTimeServiceTrigger(TimeSpan recurrence, DateTime startingTime)
        {
            return TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(s =>
                    s.WithIntervalInSeconds(Convert.ToInt32(recurrence.TotalSeconds))
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(
                            startingTime.Hour,
                            startingTime.Minute)
                        )).Build();
        }

        /// <summary>
        /// Build test trigger
        /// </summary>
        /// <param name="execute">Parameters for execution</param>
        /// <returns>Test trigger</returns>
        private static ITrigger BuildTestTrigger(Workload execute)
        {
            execute.Program.Recurrence = execute.Program.Test!.Recurrence;

            return TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(s =>
                    s.WithIntervalInSeconds(Convert.ToInt32(execute.Program.Recurrence!.Value.TotalSeconds))
                        .StartingDailyAt(new TimeOfDay(
                            DateTime.Now.Hour,
                            DateTime.Now.Minute,
                            DateTime.Now.Second)
                        )).Build();
        }
    }
}
