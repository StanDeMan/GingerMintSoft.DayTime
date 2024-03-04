using GingerMintSoft.DayTime;
using Newtonsoft.Json;
using Quartz;
using JobTask = System.Threading.Tasks.Task;

namespace DayTimeService.Daily.Jobs
{
    public class DailyJob : IJob
    {
        private readonly ILogger<SunRiseSunSetJob> _logger = LoggerFactory
            .Create(logging => logging.AddConsole())
            .CreateLogger<SunRiseSunSetJob>();

        public async JobTask Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var execute = JsonConvert.DeserializeObject<Workload>(dataMap.GetString("Execute")!);

            //actual date and time set to midnight
            var now = DateTime.Now.ToLocalTime();
            var actDate = new DateTime(now.Year, now.Month, now.Day);

            var day = execute!.Program.Test!.Active 
                ? CreateTestExecutionTimes(execute) 
                : Calculate.SunRiseSunSet(actDate, execute);

            // for testing replace execution values 
            if(execute.Program.Test!.Active)
            {
                day.SunRise = DateTime.Now + execute.Program.Test.First!.Value;
                day.SunSet = DateTime.Now + execute.Program.Test.Second!.Value;
            }

            LogDailyJob(execute, day);

            var scheduler = await SchedulerBuilder.Create().Build().GetScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<SunRiseSunSetJob>().Build();

            var triggers = execute.Program.Tasks.OrderBy(tsk => tsk.Id)
                .Select(tasksToExec => (ISimpleTrigger)TriggerBuilder.Create()
                    .StartAt(tasksToExec.Id == Convert.ToInt32(DayTimeServiceWorker.EnmDay.SunRise)
                        ? day.SunRise
                        : day.SunSet)
                    .ForJob(job)
                    .UsingJobData("Command", tasksToExec.Command)
                    .UsingJobData("TaskId", tasksToExec.TaskId)
                    .WithIdentity(tasksToExec.TaskId)
                    .Build())
                .Cast<ITrigger>()
                .ToList();

            await scheduler.ScheduleJob(job, triggers, true);
        }

        /// <summary>
        /// Log daily job times
        /// </summary>
        /// <param name="execute">Execution parameters</param>
        /// <param name="day">Actual day data</param>
        private void LogDailyJob(Workload execute, Day day)
        {
            _logger.LogInformation(
                "DayTimeServiceWorker executing at sunrise {time} and at sunset: {time}. Test status: {bool}",
                day.SunRise,
                day.SunSet,
                execute.Program.Test!.Active);
        }

        /// <summary>
        /// Create test sun rise and sun set times
        /// </summary>
        /// <param name="execute">Read test data from DailyWorkload.json</param>
        /// <returns>Test executing Times for actual day</returns>
        private static Day CreateTestExecutionTimes(Workload execute)
        {
            return new Day
            {
                SunRise = DateTime.Now + execute.Program.Test!.First!.Value,
                SunSet = DateTime.Now + execute.Program.Test.Second!.Value
            };
        }
    }
}
