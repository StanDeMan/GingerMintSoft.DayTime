namespace GingerMintSoft.DayTime.Scheduler
{
    /// <summary>
    /// Features: Fast, Small, doesn't poll, Recurring Tasks
    /// </summary>
    public class TaskScheduler : IDisposable
    {
        private readonly TaskCollection _taskQueue = new();
        private readonly AutoResetEvent _autoResetEvent = new(false);
        private Thread? _thread;

        public bool Started { get; private set; }

        /// <summary>
        /// Start running tasks
        /// </summary>
        public void Start()
        {
            lock (_taskQueue)
            {
                if (Started) return;

                Started = true;
                _thread = new Thread(Run);
                _thread.Start();
            }
        }

        public void Stop()
        {
            WriteLog("Task Scheduler thread stopping");
            Started = false;            
            _autoResetEvent.Set();
            WriteLog("AutoResetEvent set called");
            _thread!.Join();
            WriteLog("Task Scheduler thread stopped");
        }

        public void Dispose()
        {
            Stop();
            _autoResetEvent.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task">Once ITask object is added, it should never be updated from outside TaskScheduler</param>
        public void AddTask(ITask? task)
        {
            ITask? earliestTask;

            lock(_taskQueue)
            {
                earliestTask = GetEarliestScheduledTask();                
                _taskQueue.Add(task);
            }
            WriteLog("Added task # " + task!.TaskId);

            if (earliestTask != null && task.StartTime >= earliestTask.StartTime) return;

            _autoResetEvent.Set();
            WriteLog("AutoResetEvent is Set");
        }

        public void AddTask(Action taskAction, DateTime startTime)
        {
            AddTask(new RecurringTask(taskAction, startTime, TimeSpan.Zero));
        }

        public void AddTask(Action taskAction, DateTime startTime, TimeSpan recurrence)
        {
            AddTask(new RecurringTask(taskAction, startTime, recurrence));
        }

        private void ReScheduleRecurringTask(ITask? task)
        {
            var nextRunTime = task!.GetNextRunTime(task.StartTime);

            if (nextRunTime == DateTime.MinValue) return;

            task.StartTime = nextRunTime;
            lock (_taskQueue)
                _taskQueue.Add(task);
            WriteLog("Recurring task # " + task.TaskId + " scheduled for " + task.StartTime);
        }

        private ITask? GetEarliestScheduledTask()
        {
            lock(_taskQueue)
            {
                return _taskQueue.First();
            }
        }

        public int TaskCount => _taskQueue.Count;

        public bool RemoveTask(ITask? task)
        {
            WriteLog("Removing task # " + task!.TaskId);

            lock(_taskQueue)
                return _taskQueue.Remove(task);
        }

        public bool RemoveTask(string taskId)
        {
            lock(_taskQueue)
                return _taskQueue.Remove(_taskQueue.First(n => n.TaskId == taskId));
        }

        public bool UpdateTask(ITask? task, DateTime startTime)
        {
            WriteLog("Updating task # " + task!.TaskId + " (Remove & Add)");

            lock(_taskQueue)
            {
                if (!RemoveTask(task)) return false;

                task.StartTime = startTime;
                AddTask(task);

                return true;
            }
        }

        private void Run()
        {
            WriteLog("Task Scheduler thread starting");
            var tolerance = TimeSpan.FromSeconds(1);

            while (Started)
            {
                try
                {
                    var task = GetEarliestScheduledTask();

                    if(task != null)
                    {
                        if (task.StartTime - DateTime.Now < tolerance)
                        {
                            WriteLog("Starting task " + task.TaskId);
                            try
                            {
                                task.Run();
                            }
                            catch (Exception e)
                            {
                                WriteLog("Exception while running Task # " + task.TaskId);
                                WriteLog(e.ToString());
                            }
                            WriteLog("Completed task " + task.TaskId);
                            lock (_taskQueue) _taskQueue.Remove(task);
                            ReScheduleRecurringTask(task);
                        }
                        else
                        {
                            var waitTime = (task.StartTime - DateTime.Now);
                            var min15 = TimeSpan.FromMinutes(15);

                            if (waitTime > min15) waitTime = min15; 

                            WriteLog("Scheduler thread waiting for " + waitTime.ToString());
                            _autoResetEvent.WaitOne(waitTime);
                            WriteLog("Scheduler thread awakening from sleep " + waitTime.ToString());
                        }
                    }
                    else
                    {
                        WriteLog("Scheduler thread waiting indefinitely");
                        _autoResetEvent.WaitOne();
                        WriteLog("Scheduler thread awakening from indefinite sleep");
                    }
                }
                catch (Exception e)
                {
                    WriteLog("Exception: " + e);
                } 
            }
                
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void WriteLog(string message)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + ": " + message);
        }
    }
}
