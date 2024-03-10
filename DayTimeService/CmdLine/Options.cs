using CommandLine;
using CommandLine.Text;

namespace DayTimeService.CmdLine
{
    public class Options
    {
        [Option('f', "filename", Required = false, HelpText = "Input filename.")]
        public string? Filename { get; set; }

        [Usage(ApplicationAlias = "DayTime")]
        public static IEnumerable<Example> Examples => 
        [
            new Example("Load workload from json file", new Options { Filename = "DailyWorkload.json" })
        ];
    }
}
