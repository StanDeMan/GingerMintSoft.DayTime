namespace GingerMintSoft.DayTime
{
    /// <summary>
    /// Utility class for triggering an event every 24 hours at a specified time of day
    /// </summary>
    /// <remarks>
    /// Initiator
    /// </remarks>
    /// <param name="hour">The hour of the day to trigger</param>
    /// <param name="minute">The minute to trigger</param>
    /// <param name="second">The second to trigger</param>
    public sealed class DailyTrigger(int hour, int minute = 0, int second = 0) : IDisposable
    {
        /// <summary>
        /// Time of day (from 00:00:00) to trigger
        /// </summary>
        private TimeSpan TriggerTime { get; } = new TimeSpan(hour, minute, second);

        /// <summary>
        /// Task cancellation token source to cancel delayed task on disposal
        /// </summary>
        private CancellationTokenSource? CancellationToken { get; set; } = new CancellationTokenSource();

        /// <summary>
        /// Reference to the running task
        /// </summary>
        private Task? RunningTask { get; set; }

        public void Execute()
        {            
            RunningTask = Task.Run(async () => 
            {
                while (true)
                {
                    var triggerTime = DateTime.Today + TriggerTime - DateTime.Now;

                    if (triggerTime < TimeSpan.Zero)
                    {
                        triggerTime = triggerTime.Add(new TimeSpan(24, 0, 0));
                    }
                    
                    await Task.Delay(triggerTime, CancellationToken!.Token);
                    OnTimeTriggered?.Invoke();
                    await CancellationToken.CancelAsync();

                    return;
                }
            }, CancellationToken!.Token);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            CancellationToken?.Cancel();
            CancellationToken?.Dispose();
            RunningTask?.Dispose();
        }

        /// <summary>
        /// Triggers once every 24 hours on the specified time
        /// </summary>
        public event Action? OnTimeTriggered;

        /// <summary>
        /// Finalized to ensure Dispose is called when out of scope
        /// </summary>
        ~DailyTrigger() => Dispose();
    }
}
