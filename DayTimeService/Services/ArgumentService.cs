using System.Text;
using CommandLine;
using DayTimeService.CmdLine;

namespace DayTimeService.Services
{
    public class ArgumentService(ParserResult<Options> args)
    {
        private readonly Arguments _arguments = new();

        public Arguments Read()
        {
            try
            {
                _arguments.WorkloadFile = args.Value.Filename;
            }
            catch (Exception)
            {
                _arguments.Errors = args.Errors;
            }

            return _arguments;
        }
    }

    public class Arguments
    {
        public string WorkloadFileDefaultName { get; private set; } = new StringBuilder().Append("DailyWorkload.json").ToString();
        public string? WorkloadFile { get; set; }
        public IEnumerable<Error>? Errors { get; set; }
    }
}
