using DayTimeService.Daily;
using DayTask = DayTimeService.Daily.Task;

namespace DayTimeService.Test
{
    [TestClass]
    public class Calculations
    {
        [TestMethod]
        public void TestCalculateWithTooLargeMinMaxBounds()
        {
            var workload = new Workload
            {
                Program = new Daily.Program
                {
                    TaskId = "TestWorkload",
                    InputSink = "GpioPath",
                    Coordinate = new Coordinate
                    {
                        Latitude = 48.11,
                        Longitude = 7.91
                    }, 
                    Tasks = new List<DayTask> 
                        {
                        new()
                        {
                            Id = 0,
                            TaskId = "Test1",
                            Offset = 121,
                            Command = "Command 1"
                        },
                
                        new()
                        {
                            Id = 1,
                            TaskId = "Test2",
                            Offset = -121,
                            Command = "Command 1"
                        }
                    }
                }
            };

            // take this to get a wanted response
            var now = new DateTime(2024, 3, 4);
            var actDate = new DateTime(now.Year, now.Month, now.Day);

            var dayData= Calculate.SunRiseSunSet(actDate, workload);

            Assert.IsNotNull(dayData);
            Assert.IsTrue(dayData.DayLength is { Hours: 11, Minutes: 15 });
            Assert.IsTrue(dayData.SunRise is { Hour: 9, Minute: 2, Second: 21 });
            Assert.IsTrue(dayData.SunSet is { Hour: 16, Minute: 17, Second: 32 });
        }

        [TestMethod]
        public void TestCalculateWithMinMaxBounds()
        {
            var workload = new Workload
            {
                Program = new Daily.Program
                {
                    TaskId = "TestWorkload",
                    InputSink = "GpioPath",
                    Coordinate = new Coordinate()
                    {
                        Latitude = 48.11,
                        Longitude = 7.91
                    },
                    Tasks = new List<DayTask>
                    {
                        new()
                        {
                            Id = 0,
                            TaskId = "Test1",
                            Offset = 120,
                            Command = "Command 1"
                        },
                        new()
                        {
                            Id = 1,
                            TaskId = "Test2",
                            Offset = -120,
                            Command = "Command 1"
                        }
                    }
                }
            };

            // take this to get a wanted response
            var now = new DateTime(2024, 3, 4);
            var actDate = new DateTime(now.Year, now.Month, now.Day);

            var dayData = Calculate.SunRiseSunSet(actDate, workload);

            Assert.IsNotNull(dayData);
            Assert.IsTrue(dayData.DayLength is { Hours: 11, Minutes: 15 });
            Assert.IsTrue(dayData.SunRise is { Hour: 9, Minute: 2, Second: 21 });
            Assert.IsTrue(dayData.SunSet is { Hour: 16, Minute: 17, Second: 32 });
        }

        [TestMethod]
        public void TestCalculateWithOneHourBounds()
        {
            var workload = new Workload
            {
                Program = new Daily.Program
                {
                    TaskId = "TestWorkload",
                    InputSink = "GpioPath",
                    Coordinate = new Coordinate()
                    {
                        Latitude = 48.11,
                        Longitude = 7.91
                    },
                    Tasks = new List<DayTask>
                    {
                        new()
                        {
                            Id = 0,
                            TaskId = "Test1",
                            Offset = 60,
                            Command = "Command 1"
                        },
                        new()
                        {
                            Id = 1,
                            TaskId = "Test2",
                            Offset = -60,
                            Command = "Command 1"
                        }
                    }
                }
            };

            // take this to get a wanted response
            var now = new DateTime(2024, 3, 4);
            var actDate = new DateTime(now.Year, now.Month, now.Day);

            var dayData = Calculate.SunRiseSunSet(actDate, workload);

            Assert.IsNotNull(dayData);
            Assert.IsTrue(dayData.DayLength is { Hours: 11, Minutes: 15, Seconds: 11 });
            Assert.IsFalse(dayData.SunRise is { Hour: 9, Minute: 2, Second: 21 });
            Assert.IsFalse(dayData.SunSet is { Hour: 16, Minute: 17, Second: 32 });

            // test for one our later
            Assert.IsTrue(dayData.SunRise is { Hour: 8, Minute: 2, Second: 21 });
            Assert.IsTrue(dayData.SunSet is { Hour: 17, Minute: 17, Second: 32 });
        }

        [TestMethod]
        public void TestCalculateWithEmptyOffsetBounds()
        {
            var workload = new Workload
            {
                Program = new Daily.Program
                {
                    TaskId = "TestWorkload",
                    InputSink = "GpioPath",
                    Coordinate = new Coordinate()
                    {
                        Latitude = 48.11,
                        Longitude = 7.91
                    },
                    Tasks = new List<DayTask>
                    {
                        new()
                        {
                            Id = 0,
                            TaskId = "Test1",
                            Command = "Command 1"
                        },
                        new()
                        {
                            Id = 1,
                            TaskId = "Test2",
                            Command = "Command 1"
                        }
                    }
                }
            };

            // take this to get a wanted response
            var now = new DateTime(2024, 3, 4);
            var actDate = new DateTime(now.Year, now.Month, now.Day);

            var dayData = Calculate.SunRiseSunSet(actDate, workload);

            Assert.IsNotNull(dayData);
            Assert.IsTrue(dayData.DayLength is { Hours: 11, Minutes: 15, Seconds: 11 });

            // correct times without offset
            Assert.IsTrue(dayData.SunRise is { Hour: 7, Minute: 2, Second: 21 });
            Assert.IsTrue(dayData.SunSet is { Hour: 18, Minute: 17, Second: 32 });
        }
    }
}