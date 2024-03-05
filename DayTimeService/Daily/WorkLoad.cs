using Newtonsoft.Json;

namespace DayTimeService.Daily
{
    public class Application
    {
        /// <summary>
        /// Read DailyWorkload.json file
        /// and build belonging object
        /// </summary>
        /// <param name="pathFile">Path and file to DailyWorkload.json</param>
        /// <returns>Filled Workload object</returns>
        public Workload? ReadWorkload(string pathFile)
        {
            using StreamReader reader = new(pathFile);
            var json = reader.ReadToEnd();
            var workLoad = JsonConvert.DeserializeObject<Workload>(json);
        
            return workLoad;
        }
    }

    public class Workload
    {
        public required Program Program { get; set; }
    }

    public class Program
    {
        public required string TaskId { get; set; }
        public TimeSpan? Recurrence { get; set; }
        public required Coordinate Coordinate { get; set; }
        public required List<Task> Tasks { get; set; }
        public Test? Test { get; set; }
    }

    public class Coordinate
    {
        public required double Latitude { get; set; }
        public required double Longitude { get; set; }
    }

    public class Task
    {
        public required int Id { get; set; }
        public required string TaskId { get; set; }
        public double Offset { get; set; }
        public required string Command { get; set; }
    }

    public class Test
    {
        public bool Active { get; set; }
        public TimeSpan? Recurrence { get; set; }
        public TimeSpan? First { get; set; }
        public TimeSpan? Second { get; set; }
    }
}
