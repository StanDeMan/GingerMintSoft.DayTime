using System.Text;

namespace DayTimeService.Services
{
    public class ArgumentService(IReadOnlyList<string?> args)
    {
        private readonly Arguments _arguments = new();

        public Arguments Read()
        {
            try
            {
                _arguments.WorkloadFile = args[0]!.Contains(".json", StringComparison.OrdinalIgnoreCase)     
                    ? args[0]                                           // take json file
                    : null;                                             // no file      

                _arguments.UseDefaultWorkload = args[1];
            }
            catch (Exception)
            {
                _arguments.WorkloadFile = null;
                _arguments.UseDefaultWorkload = "default";
            }

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
