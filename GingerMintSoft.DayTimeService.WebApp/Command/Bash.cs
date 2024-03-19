using System.Diagnostics;
using GingerMintSoft.DayTimeService.WebApp.Hardware;

namespace GingerMintSoft.DayTimeService.WebApp.Command
{
    public sealed class Bash
    {
        private static readonly ILogger<Bash> Logger = LoggerFactory
            .Create(logging => logging.AddConsole())
            .CreateLogger<Bash>();

        public static string Execute(string command)
        {
            var proc = Process(command);
            proc.Start();
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd();
        }

        public static async Task<string> ExecuteAsync(string command, double secsTimeout = 2)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(secsTimeout));
            var proc = Process(command);
            await proc.WaitForExitAsync(cts.Token);

            return await proc.StandardOutput.ReadToEndAsync(cts.Token);
        }

        private static Process Process(string command)
        {
            command = command.Replace("\"", "\"\"");

            var proc = new Process
            {
                StartInfo = Platform.OperatingSystem == Platform.EnmOperatingSystem.Linux
                    ? StartProcess(Platform.ProgramPath, "-c \"" + command + "\"")
                    : StartProcess("cmd.exe", $"""/c echo "{command}">> {Platform.ProgramPath}""")
            };

            proc.Start();

            // log executed command
            Logger.LogInformation($"Executed: {proc.StartInfo.FileName} {proc.StartInfo.Arguments}");

            return proc;
        }

        private static ProcessStartInfo StartProcess(string? fileName, string arguments)
        {
            return new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
        }
    }
}
