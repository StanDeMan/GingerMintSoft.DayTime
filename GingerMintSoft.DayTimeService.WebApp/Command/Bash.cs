using System.Diagnostics;
using GingerMintSoft.DayTimeService.WebApp.Hardware;

namespace GingerMintSoft.DayTimeService.WebApp.Command
{
    public sealed class Bash
    {
        private static readonly ILogger<Bash> Logger = LoggerFactory
            .Create(logging => logging.AddConsole())
            .CreateLogger<Bash>();

        /// <summary>
        /// Execute command via process
        /// </summary>
        /// <param name="command">Execute this command</param>
        /// <returns>Return from standard output</returns>
        public static string Execute(string command)
        {
            using var proc = Process(command);
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd();
        }

        /// <summary>
        /// Execute command via process
        /// </summary>
        /// <param name="command">Execute this command</param>
        /// <param name="secsTimeout">Execute in this time range</param>
        /// <returns>Return from standard output</returns>
        public static async Task<string> ExecuteAsync(string command, double secsTimeout = 2)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(secsTimeout));
            using var proc = Process(command);
            await proc.WaitForExitAsync(cts.Token);

            return await proc.StandardOutput.ReadToEndAsync(cts.Token);
        }

        /// <summary>
        /// Process the command
        /// </summary>
        /// <param name="command">This command</param>
        /// <returns>Command process</returns>
        private static Process Process(string command)
        {
            command = command.Replace("\"", "\"\"");

            var proc = new Process
            {
                StartInfo = Platform.OperatingSystem == Platform.EnmOperatingSystem.Linux
                    ? ProcessStartInfo(Platform.ProgramPath, "-c \"" + command + "\"")
                    : ProcessStartInfo("cmd.exe", $"""/c echo "{command}">> {Platform.ProgramPath}""")
            };

            proc.Start();

            // log executed command
            Logger.LogInformation($"Executed: {proc.StartInfo.FileName} {proc.StartInfo.Arguments}");

            return proc;
        }

        /// <summary>
        /// Start the shell process
        /// Linux: execute with bash
        /// Windows: execute cmd.exe
        /// </summary>
        /// <param name="fileName">Which shell to use</param>
        /// <param name="arguments">shell commands</param>
        /// <returns>Start info for process</returns>
        private static ProcessStartInfo ProcessStartInfo(string? fileName, string arguments)
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
