using DayTimeService.Execute;
using Quartz;
using JobTask = System.Threading.Tasks.Task;

namespace DayTimeService.Daily.Jobs
{
    public class SunRiseSunSetJob : IJob
    {
        private readonly ILogger<SunRiseSunSetJob> _logger = LoggerFactory
            .Create(logging => logging.AddConsole())
            .CreateLogger<SunRiseSunSetJob>();

        public JobTask Execute(IJobExecutionContext context)
        {
            var dataMap = context.MergedJobDataMap;
            var taskId = dataMap.GetString("TaskId");
            var command = dataMap.GetString("Command");

            var bOk = Command.Execute(command!);

            LogSunRiseSunSetJob(taskId, command, bOk);

            return JobTask.CompletedTask;
        }

        /// <summary>
        /// Log sun rise and sun set times
        /// </summary>
        /// <param name="taskId">This task is executed</param>
        /// <param name="command">Command to execute</param>
        /// <param name="bOk">Status of command execution</param>
        private void LogSunRiseSunSetJob(string? taskId, string? command, bool bOk)
        {
            _logger.LogInformation(
                "DayTimeServiceWorker executing Task {string} with command: {string} at {time}. Executed: {bool}",
                taskId,
                command,
                DateTimeOffset.Now.ToLocalTime(),
                bOk);
        }
    }
}
