using System.Reflection;
using DayTimeService.Daily;
using DayTimeService.Execute;
using DayTimeService.Hardware;
using GingerMintSoft.DayTime.Scheduler;
using Quartz;
using Quartz.Impl;
using Task = System.Threading.Tasks.Task;
using DayTimeTask = GingerMintSoft.DayTime.Scheduler.Task;

namespace DayTimeService
{
    public class DayTimeServiceWorker(ILogger<DayTimeServiceWorker> logger) : BackgroundService
    {
        public enum Day { SunRise = 0, SunSet = 1, Undefined = 9999 }
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

            var scheduler = await new StdSchedulerFactory().GetScheduler(stoppingToken);
            await scheduler.Start(stoppingToken);
 
            var job = JobBuilder.Create<DayTimeJob>()
                .WithIdentity(execute.Program.TaskId)
                //.UsingJobData("someKey", execute)
                .Build();
 
            var trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(s => 
                    s.WithIntervalInHours(Convert.ToInt32(recurrence.TotalHours))
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(
                            startingTime.Hour, 
                            startingTime.Minute))
                    ).Build();

            await scheduler.ScheduleJob(job, trigger, stoppingToken);

            var recurringTask = new RecurringTask(() =>
                {
                    //actual date and time set to midnight
                    var now = DateTime.Now.ToLocalTime();
                    var actDate = new DateTime(now.Year, now.Month, now.Day);

                    var day = Define.CalcSunRiseSunSet(actDate, execute);

                    foreach (var taskToExec in execute.Program.Tasks.OrderBy(tsk => tsk.Id))
                    {
                        //AddTask(new DayTimeTask()
                        //{
                        //    TaskId = taskToExec.TaskId,
                        //    StartTime = taskToExec.Id == Convert.ToInt32(Day.SunRise)
                        //        ? day.SunRise
                        //        : day.SunSet,
                        //    TaskAction = () =>
                        //    {
                        //        var bOk = Command.Execute(taskToExec.Command);

                        //        _logger.LogInformation(
                        //            "DayTimeServiceWorker executing Task {string} with command: {string} at {time}. Executed: {bool}",
                        //            taskToExec.Command,
                        //            taskToExec.TaskId,
                        //            DateTimeOffset.Now.ToLocalTime(),
                        //            bOk);
                        //    }
                        //});
                    }
                },
                startingTime,
                recurrence,
                execute.Program.TaskId);

            while (!stoppingToken.IsCancellationRequested)
            {
                // do some blinking here
                Command.Execute((_ledOn ? "w 14 1" : "w 14 0"));
                _ledOn = !_ledOn;

                await Task.Delay(250, stoppingToken);
            }
        }
    }

    public class DayTimeJob : IJob
    {
        public Workload? Workload { get; set; }

        public Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            //TypeT someValue = <TypeT>dataMap.Get("someKey");
            return Task.CompletedTask;
        }
    }
}
