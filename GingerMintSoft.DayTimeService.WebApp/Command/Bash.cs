using System.Diagnostics;
using GingerMintSoft.DayTimeService.WebApp.Hardware;

namespace GingerMintSoft.DayTimeService.WebApp.Command
{
    public sealed class Bash
    {
        public static string Execute(string command)
        {
            var proc = Process(command);
            proc.Start();
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd();
        }

        public static async Task<string> ExecuteAsync(string command, double secsTimeout = 10)
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
                    ? new ProcessStartInfo
                    {
                        FileName = Platform.ProgramPath,
                        Arguments = "-c \"" + command + "\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                    : new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"""/c echo "{command}">> {Platform.ProgramPath}""",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                    }
            };

            proc.Start();
            proc.WaitForExit();

            return proc;
        }
    }
}
