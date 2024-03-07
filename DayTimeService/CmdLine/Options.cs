using CommandLine;
using CommandLine.Text;

namespace DayTimeService.CmdLine
{
    public class Options
    {
        [Option("filename", Required = false, HelpText = "Input filename.")]
        public string? Filename { get; set; }

        [Usage(ApplicationAlias = "DayTime")]
        public static IEnumerable<Example> Examples => 
        [
            new Example("Calculates sun rise and sun set times using the data from this file.", new Options { Filename = "DailyWorkload.json" })
        ];
    }
}
