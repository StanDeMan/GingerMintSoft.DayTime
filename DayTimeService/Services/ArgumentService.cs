using System.Globalization;

namespace DayTimeService.Services
{
    public class ArgumentService(IReadOnlyList<string?> args)
    {
        private readonly Arguments _arguments = new();

        public Arguments Read()
        {
            _arguments.WorkloadFile = args[0];
            _arguments.StartTime = DateTime.ParseExact(args[1]!, "dd-MM-yyyy-HH:mm", CultureInfo.CreateSpecificCulture("DE-de"));

            return _arguments;
        }
    }

    public class Arguments
    {
        public string? WorkloadFile { get; set; }
        public DateTime? StartTime { get; set; }
    }
}
