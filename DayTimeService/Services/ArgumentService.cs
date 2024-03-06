using System.Text;

namespace DayTimeService.Services
{
    public class ArgumentService(IReadOnlyList<string?> args)
    {
        private readonly Arguments _arguments = new();

        public Arguments Read()
        {
            _arguments.WorkloadFile = args[0]
                !.Contains(".json", StringComparison.OrdinalIgnoreCase)     // check if json file
                ? args[0] : "";                                             // no file      

            _arguments.UseDefaultWorkload = args[1];

            return _arguments;
        }
    }

    public class Arguments
    {
        public string WorkloadFileDefaultName { get; private set; } = new StringBuilder().Append("DailyWorkload.json").ToString();
        public string? WorkloadFile { get; set; }
        public string? UseDefaultWorkload { get; set; }
    }
}
