using CommandLine;
using CommandLine.Text;

namespace DayTimeService.CmdLine
{
    public class Options
    {
        [Option('f', "filename", Required = false, HelpText = "Input filename.")]
        public string? Filename { get; private init; }

        [Usage(ApplicationAlias = "DayTime")]
        public static IEnumerable<Example> Examples =>
            new List<Example>() 
            {
                new Example("Load workload from json file", new Options { Filename = "DailyWorkload.json" })
            };
    }
}
