namespace GingerMintSoft.DayTime.Scheduler
{
    public class RecurringTask : ITask
    {
        public string? TaskId { get; set; }
        public DateTime StartTime { get; set; }
        public TaskCollection Tasks { get; set; } = new();

        public Action TaskAction { get; set; }

        /// <summary>
        /// TimeSpan.Zero mean null
        /// </summary>
        public TimeSpan Recurrence { get; set; }

        public RecurringTask(Action taskAction, DateTime startTime, TimeSpan recurrence, string? taskId = null!)
        {
            TaskAction = taskAction;
            StartTime = startTime;
            Recurrence = recurrence;
            TaskId = taskId;
        }

        public void Run()
        {
            TaskAction();
        }

        public DateTime GetNextExecutionTime(DateTime lastExecutionTime)
        {
            return Recurrence != TimeSpan.Zero
                ? lastExecutionTime.Add(Recurrence)
                : DateTime.MinValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task">Once ITask object is added, it should never be updated from outside TaskScheduler</param>
        public void AddTask(ITask? task)
        {
            Tasks.Add(task);
        }
    }
}
