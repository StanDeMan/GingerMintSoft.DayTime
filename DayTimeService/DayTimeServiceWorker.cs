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
        private bool _ledOn;
        public ArgumentService Arguments { get; } = arguments;

        public enum EnmDay
        {
            // ReSharper disable once UnusedMember.Global
            Undefined = -1, 
            SunRise = 0, 
            SunSet = 1
        }

        public enum EnmInstruction
        {
            // ReSharper disable once UnusedMember.Global
            Undefined = -1,
            On = 0,
            Off = 1,
            Blink = 2
        }

        /// <summary>
        /// Long term service:
        /// Switch pv accumulator storage by sun rise and sun set 
        /// times calculated by geo coordinates read from a json
        /// </summary>
        /// <param name="stoppingToken">Stopping long term service</param>
        /// <returns>Exit code</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const int blinkError = 100;
            const int blinkNormal = 250;

            Workload? execute = null;

            try
            {
                execute = await DayTimeScheduler(stoppingToken);
            }
            catch (Exception ex)
            {
                // show all collected errors
                logger.LogError("DayTimeServiceWorker.ExecuteAsync error: {string}", ex);

                if (Arguments.Errors!.Any())
                {
                    foreach (var error in Arguments.Read().Errors!)
                    {
                        logger.LogError("DayTimeServiceWorker.ExecuteAsync arguments error(s): {string}", error);
                    }
                }
            }

            var (ledOn, ledOff) = ReadLedInstructions(execute!);

            while (!stoppingToken.IsCancellationRequested)
            {
                // do some blinking here
                Command.Execute(((_ledOn ? ledOn : ledOff)!));
                _ledOn = !_ledOn;

                await Task.Delay(Arguments.Errors!.Any() 
                    ? blinkError 
                    : blinkNormal, 
                    stoppingToken);
            }
        }

        /// <summary>
        /// Extract led on/off instruction
        /// </summary>
        /// <param name="execute">Workload with instruction</param>
        /// <returns>ledOn and ledOff instructions</returns>
        private static (string?, string?) ReadLedInstructions(Workload execute)
        {
            var instructions = execute.Program!.Tasks[(int)EnmInstruction.Blink].Instructions;
            var ledOn = instructions!.Find(on => on.Id == (int)EnmInstruction.On)!.Command;
            var ledOff = instructions.Find(on => on.Id == (int)EnmInstruction.Off)!.Command;

            return (ledOn, ledOff);
        }

        /// <summary>
        /// Switch pv accumulator storage by sun rise and
        /// sun set times calculated by geo coordinates
        /// </summary>
        /// <param name="stoppingToken">Let the service know if it should stop</param>
        /// <returns>Exit code</returns>
        private async Task<Workload> DayTimeScheduler(CancellationToken stoppingToken)
        {
            logger.LogInformation(
                "DayTimeServiceWorker started at: {time}",
                DateTimeOffset.Now.ToLocalTime());

            var workloadFile = Arguments.Read().WorkloadFile ?? Arguments.Read().DefaultWorkloadFile;
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var execute = new Application().ReadWorkload(Platform.OperatingSystem == Platform.EnmOperatingSystem.Windows 
                ? $@"{currentPath}\{workloadFile}"
                : $"{currentPath}/{workloadFile}");

            // if the workload file cannot be found we take the defaults
            execute ??= new Application().ReadDefaultWorkload();

            // start calculate sun rise/set every midnight after 5 seconds
            var startingTime = DateTime.Today.AddDays(1).AddSeconds(5);
            var recurrence = execute!.Program!.Recurrence ?? TimeSpan.FromDays(1);

            logger.LogInformation(
                "DayTimeServiceWorker will be executed at: {time} with recurrence of {double:F} hours",
                startingTime,
                recurrence.TotalHours);

            await StartDayTimeScheduler(stoppingToken, execute, recurrence, startingTime);

            return execute;
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
                .WithIdentity(execute.Program!.TaskId)
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
            return execute.Program!.Test is { Active: true }
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
            execute.Program!.Recurrence = execute.Program.Test!.Recurrence;

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
