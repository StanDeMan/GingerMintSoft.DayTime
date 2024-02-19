using GingerMintSoft.DayTime.Scheduler;
using Task = GingerMintSoft.DayTime.Scheduler.Task;
using TaskScheduler = GingerMintSoft.DayTime.Scheduler.TaskScheduler;

namespace GingerMintSoft.DayTimeTest
{
    [TestClass]
    public class DayTimeSchedulerTest
    {
        private int _triggerCnt;
        private const string TaskNumber = "1";

        [TestMethod]
        public void TestSchedulerRecurringTaskThreeTimes()
        {
            var scheduler = new TaskScheduler();
            scheduler.Start();

            var task = new RecurringTask(
                () =>
                {
                    if (++_triggerCnt != 3) return;

                    scheduler.Dispose();
                    Assert.IsTrue(_triggerCnt == 3);
                },
                DateTime.Now.AddSeconds(1),
                TimeSpan.FromSeconds(1)
            )
            {
                TaskId = TaskNumber
            };

            scheduler.AddTask(task);
        }

        [TestMethod]
        public void TestSchedulerTaskThreeTimesWithRecurringSet()
        {
            var scheduler = new TaskScheduler();
            scheduler.Start();

            scheduler.AddTask(new Task()
            {
                StartTime = DateTime.Now.AddSeconds(1),
                TaskAction = () =>
                {
                    if (++_triggerCnt != 3) return;

                    scheduler.Dispose();
                    Assert.IsTrue(_triggerCnt == 1);
                },
                Recurrance = TimeSpan.FromSeconds(1)
            });
        }

        [TestMethod]
        public void TestSchedulerTaskSingleShot()
        {
            var scheduler = new TaskScheduler();
            scheduler.Start();

            scheduler.AddTask(new Task()
            {
                TaskId = "StartThis",
                StartTime = DateTime.Now.AddSeconds(1),
                TaskAction = () =>
                {
                    if (++_triggerCnt != 1) return;

                    scheduler.Dispose();
                    Assert.IsTrue(_triggerCnt == 1);
                }
            });
        }

        [TestMethod]
        public void TestSchedulerTwoSingleTaskShots()
        {
            var scheduler = new TaskScheduler();
            scheduler.Start();

            var firstTask = new Task()
            {
                TaskId = "StartFirst",
                StartTime = DateTime.Now.AddSeconds(1),
                TaskAction = () =>
                {
                    if (++_triggerCnt != 1) return;

                    Assert.IsTrue(_triggerCnt == 1);
                }
            };

            var secondTask = new Task()
            {
                TaskId = "StartSecond",
                StartTime = DateTime.Now.AddSeconds(3),
                TaskAction = () =>
                {
                    if (++_triggerCnt != 2) return;

                    scheduler.Dispose();
                    Assert.IsTrue(_triggerCnt == 2);
                }
            };

            scheduler.AddTask(firstTask);
            scheduler.AddTask(secondTask);
        }
    }
}
