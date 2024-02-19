namespace GingerMintSoft.DayTime.Scheduler
{
    public class RecurringTask : ITask
    {
        public string? TaskId { get; set; }

        public DateTime StartTime { get; set; }

        public Action TaskAction { get; set; }

        /// <summary>
        /// TimeSpan.Zero mean null
        /// </summary>
        public TimeSpan Recurrance { get; set; }

        public RecurringTask(Action taskAction, DateTime startTime, TimeSpan recurrence, string? taskId = null!)
        {
            TaskAction = taskAction;
            StartTime = startTime;
            Recurrance = recurrence;
            TaskId = taskId;
        }

        public void Run()
        {
            TaskAction();
        }

        public DateTime GetNextRunTime(DateTime lastExecutionTime)
        {
            return Recurrance != TimeSpan.Zero
                ? lastExecutionTime.Add(Recurrance)
                : DateTime.MinValue;
        }
    }
}
