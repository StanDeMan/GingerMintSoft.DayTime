using Newtonsoft.Json;

namespace DayTimeService.Daily
{
    public class Application
    {
        public Workload? ReadWorkload(string path)
        {
            using StreamReader reader = new(path);
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
        public required Coordinate Coordinate { get; set; }
        public required List<Task> Tasks { get; set; }
    }

    public class Coordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Task
    {
        public DateTime ExecutionDateTime { get; set; }
        public required string TaskId { get; set; }
        public required string Command { get; set; }
    }
}
