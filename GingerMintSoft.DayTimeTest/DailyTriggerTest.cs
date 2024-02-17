using System.Diagnostics;
using GingerMintSoft.DayTime;

namespace GingerMintSoft.DayTimeTest
{
    [TestClass]
    public class DailyTriggerTest
    {
        [TestMethod]
        public void TestDailyTriggerAfterFiveSeconds()
        {
            var triggered = false;
            var actTime = DateTime.UtcNow.ToLocalTime();
            var fiveSecLater = actTime.AddSeconds(5);
            var trigger = new DailyTrigger(fiveSecLater.Hour, fiveSecLater.Minute, fiveSecLater.Second); // trigger 5 sec later

            Debug.Print($"Trigger started at: {actTime}");
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            trigger.Execute();

            while (!triggered)
            {
                trigger.OnTimeTriggered += () =>
                {
                    Debug.Print($"Trigger executed: {fiveSecLater}");
                    triggered = true;

                    Thread.Sleep(500);
                };
            }

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;

            Assert.IsTrue(ts.Seconds <= 5, "Triggered in less than or 5 seconds.");
            Debug.Print($"Duration till trigger execution: {ts.Duration().Seconds}");
            Debug.Print($"Actual Time is: {DateTime.UtcNow.ToLocalTime()}");
        }
    }
}
