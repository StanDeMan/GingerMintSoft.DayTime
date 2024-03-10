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

            return JsonConvert.DeserializeObject<Workload>(json);
        }

        /// <summary>
        /// Read the workload default settings
        /// Recurrence: every day
        /// Latitude and Longitude: my home
        /// Sunrise: with offset of 60 minutes after sunrise
        /// Sunset: with offset of 60 minutes (-60) prior to sunset
        /// Blink: blink led on/off
        /// </summary>
        /// <returns>Default settings</returns>
        public Workload? ReadDefaultWorkload()
        {
            return JsonConvert.DeserializeObject<Workload>(
                """
                {
                  "Program": {
                    "Recurrence": "1:00:00:00",
                    "TaskId": "DayTimeServiceWorker",
                    "Coordinate": {
                      "Latitude": 48.10507778308992,
                      "Longitude": 7.90856839921184
                    },
                    "Tasks": [
                      {
                        "Id": 0,
                        "TaskId": "SunRise",
                        "Offset": 60,
                        "Command": "w 23 0"
                      },
                      {
                        "Id": 1,
                        "TaskId": "SunSet",
                        "Offset": -60,
                        "Command": "w 23 1"
                      },
                      {
                        "Id": 2,
                        "TaskId": "Blink",
                        "Error": 100,
                        "Normal": 250,
                        "Instructions": [
                          {
                            "Id": 0,
                            "TaskId": "On",
                            "Command": "w 14 1"
                          },
                          {
                            "Id": 1,
                            "TaskId": "Off",
                            "Command": "w 14 0"
                          }
                        ]
                      }
                    ]
                  }
                }
                """);
        }
    }

    public class Workload
    {
        public required Program? Program { get; set; }
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
        public int? Error { get; set; }
        public int? Normal { get; set; }
        public double Offset { get; set; }
        public required string Command { get; set; }
        public List<Instruction>? Instructions { get; set; }
    }

    public class Instruction
    {
        public int? Id { get; set; }
        public string? TaskId { get; set; }
        public string? Command { get; set; }
    }

    public class Test
    {
        public bool Active { get; set; }
        public TimeSpan? Recurrence { get; set; }
        public TimeSpan? First { get; set; }
        public TimeSpan? Second { get; set; }
    }
}
