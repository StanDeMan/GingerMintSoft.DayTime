using DayTimeService.Daily;

namespace DayTimeService.Test
{
    [TestClass]
    public class ApplicationWorkload
    {
        [TestMethod]
        public void TestApplicationLoadDefaultWorkloadCheckIfLoaded()
        {
            var app = new Application();
            var defaultWorkload = app.ReadDefaultWorkload();

            Assert.IsNotNull(defaultWorkload);
        }

        [TestMethod]
        public void TestApplicationLoadDefaultWorkloadCheckIfLoadedCorrectly()
        {
            var app = new Application();
            var defaultWorkload = app.ReadDefaultWorkload();

            Assert.IsNotNull(defaultWorkload);
            Assert.AreEqual("1.00:00:00", $"{defaultWorkload.Program!.Recurrence}");
            Assert.AreEqual("DayTimeServiceWorker", defaultWorkload.Program!.TaskId);

            Assert.AreEqual(3, defaultWorkload.Program!.Tasks.Count);
            Assert.AreEqual("SunRise", defaultWorkload.Program!.Tasks[0].TaskId);
            Assert.AreEqual("SunSet", defaultWorkload.Program!.Tasks[1].TaskId);
        }

        [TestMethod]
        public void TestApplicationLoadDefaultWorkloadCheckIfTimeSpanIsOneDay()
        {
            var app = new Application();
            var defaultWorkload = app.ReadDefaultWorkload();

            Assert.IsNotNull(defaultWorkload);
            Assert.AreEqual(TimeSpan.Parse("1.00:00:00"), defaultWorkload.Program!.Recurrence);
        }

        [TestMethod]
        public void TestApplicationLoadDefaultWorkloadCheckIfCommands()
        {
            var app = new Application();
            var defaultWorkload = app.ReadDefaultWorkload();

            Assert.IsNotNull(defaultWorkload);
            Assert.AreEqual(3, defaultWorkload.Program!.Tasks.Count);
            Assert.AreEqual("w 23 0", defaultWorkload.Program!.Tasks[0].Command);
            Assert.AreEqual("w 23 1", defaultWorkload.Program!.Tasks[1].Command);
            Assert.AreNotEqual("", defaultWorkload.Program!.Tasks[0].Command);
            Assert.AreNotEqual("", defaultWorkload.Program!.Tasks[1].Command);
        }
    }
}
