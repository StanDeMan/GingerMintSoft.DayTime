using Newtonsoft.Json;
using Quartz;
using JobTask = System.Threading.Tasks.Task;

namespace DayTimeService.Daily.Jobs
{
    public class DailyJob : IJob
    {
        public JobTask Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var execute = JsonConvert.DeserializeObject<Workload>(dataMap.GetString("Execute")!);

            //actual date and time set to midnight
            var now = DateTime.Now.ToLocalTime();
            var actDate = new DateTime(now.Year, now.Month, now.Day);

            var day = Define.CalcSunRiseSunSet(actDate, execute!);
            
            //var firstTask = execute!.Program.Tasks[0];


            return JobTask.CompletedTask;
        }
    }
}
