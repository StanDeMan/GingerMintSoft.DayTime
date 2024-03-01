using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
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

            var day = Define.CalcSunRiseSunSet(actDate, execute!);
            
            var scheduler = await new StdSchedulerFactory().GetScheduler();
            await scheduler.Start();

            var job = JobBuilder.Create<SunRiseSunSetJob>()
                .WithIdentity(execute!.Program.TaskId)
                .Build();

            foreach (var taskToExec in execute.Program.Tasks.OrderBy(tsk => tsk.Id))
            {
                var trigger = (ISimpleTrigger) TriggerBuilder.Create()
                    .WithIdentity(taskToExec.TaskId)
                    .StartAt(taskToExec.Id == Convert.ToInt32(DayTimeServiceWorker.Day.SunRise) 
                        ? day.SunRise 
                        : day.SunSet)   
                    .ForJob("SunRiseSunSetJob")             // identify job with name
                    .UsingJobData("Command", taskToExec.Command)
                    .UsingJobData("TaskId", taskToExec.TaskId)
                    .Build();
                
                await scheduler.ScheduleJob(job, trigger);
            }
        }
    }
}
