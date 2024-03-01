using DayTimeService.Execute;
using DayTimeService.Logging;
using Quartz;
using JobTask = System.Threading.Tasks.Task;

namespace DayTimeService.Daily.Jobs
{
    public class SunRiseSunSetJob : IJob
    {
        private readonly ILogger _logger = Logger.LoggerFactory.CreateLogger("SunRiseSunSetJob");

        public JobTask Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var taskId = dataMap.GetString("TaskId");
            var command = dataMap.GetString("Command");

            var bOk = Command.Execute(command!);

            _logger.LogInformation(
                "DayTimeServiceWorker executing Task {string} with command: {string} at {time}. Executed: {bool}",
                taskId,
                command,
                DateTimeOffset.Now.ToLocalTime(),
                bOk);

            return JobTask.CompletedTask;
        }
    }
}
