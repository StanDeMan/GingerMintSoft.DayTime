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
                _arguments.Error = _arguments.Errors.Any();
            }

            return _arguments;
        }

        public IEnumerable<Error>? Errors
        {
            get => _arguments.Errors;
            set => _arguments.Errors = value;
        }

    }

    public class Arguments
    {
        private static readonly List<Error> Errs = new List<Error>();

        public string DefaultWorkloadFile { get; private set; } = 
            new StringBuilder().Append("DailyWorkload.json").ToString();

        public string? WorkloadFile { get; set; }

        public IEnumerable<Error>? Errors { get; set; } = Errs;

        public bool Error { get; set; }
    }
}
