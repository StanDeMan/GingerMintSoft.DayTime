using Newtonsoft.Json;
using Quartz;
using JobTask = System.Threading.Tasks.Task;

namespace DayTimeService.Daily.Jobs
{
    public class DailyJob : IJob
    {
        public async JobTask Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var execute = JsonConvert.DeserializeObject<Workload>(dataMap.GetString("Execute")!);

            //actual date and time set to midnight
            var now = DateTime.Now.ToLocalTime();
            var actDate = new DateTime(now.Year, now.Month, now.Day);

            var day = Calculate.SunRiseSunSet(actDate, execute!);

            if(execute!.Program.Test!.Active == true)
            {
                day.SunRise = DateTime.Now + execute.Program.Test.First!.Value;
                day.SunSet = DateTime.Now + execute.Program.Test.Second!.Value;
            }

            var scheduler = await SchedulerBuilder.Create().Build().GetScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<SunRiseSunSetJob>().Build();

            var triggers = execute!.Program.Tasks.OrderBy(tsk => tsk.Id)
                .Select(tasksToExec => (ISimpleTrigger)TriggerBuilder.Create()
                    .StartAt(tasksToExec.Id == Convert.ToInt32(DayTimeServiceWorker.Day.SunRise)
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
    }
}
