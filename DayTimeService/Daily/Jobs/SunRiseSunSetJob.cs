using DayTimeService.Execute;
using Quartz;
using JobTask = System.Threading.Tasks.Task;

namespace DayTimeService.Daily.Jobs
{
    public class SunRiseSunSetJob : IJob
    {
        private static readonly ILogger Logger = new Logger<SunRiseSunSetJob>(Logging.Logger.LoggerFactory);

        public JobTask Execute(IJobExecutionContext context)
        {
            var dataMap = context.MergedJobDataMap;
            var taskId = dataMap.GetString("TaskId");
            var command = dataMap.GetString("Command");

            var bOk = Command.Execute(command!);

            Logger.LogInformation(
                "DayTimeServiceWorker executing Task {string} with command: {string} at {time}. Executed: {bool}",
                taskId,
                command,
                DateTimeOffset.Now.ToLocalTime(),
                bOk);

            return JobTask.CompletedTask;
        }
    }
}
