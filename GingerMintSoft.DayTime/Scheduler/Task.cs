namespace GingerMintSoft.DayTime.Scheduler;

public class Task : ITask
{
    public string? TaskId { get; set; }
    public DateTime StartTime { get; set; }

    public Action? TaskAction { get; set; }

    /// <summary>
    /// TimeSpan.Zero mean null
    /// </summary>
    public TimeSpan Recurrence { get; set; } = TimeSpan.Zero;

    public void Run()
    {
        TaskAction!();
    }

    public DateTime GetNextRunTime(DateTime lastExecutionTime)
    {
        return Recurrence != TimeSpan.Zero
            ? lastExecutionTime.Add(Recurrence)
            : DateTime.MinValue;    }
}