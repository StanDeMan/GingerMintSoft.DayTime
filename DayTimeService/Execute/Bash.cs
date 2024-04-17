using System.Diagnostics;
using DayTimeService.Hardware;

namespace DayTimeService.Execute
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
        public static (bool, string) Execute(string command)
        {
            var ok = true;
            var responseOutput = "";

            using var proc = Process(command);

            try
            {
                proc.WaitForExit();
                responseOutput = proc.StandardOutput.ReadToEnd();
            }
            catch (Exception)
            {
                ok = false;
            }


            return (ok, responseOutput);
        }

        /// <summary>
        /// Execute command via process
        /// </summary>
        /// <param name="command">Execute this command</param>
        /// <param name="secsTimeout">Execute in this time range</param>
        /// <returns>Return from standard output</returns>
        public static async Task<(bool, string)> ExecuteAsync(string command, double secsTimeout = 2)
        {
            var ok = true;
            var responseOutput = "";

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(secsTimeout));
            using var proc = Process(command);

            try
            {
                await proc.WaitForExitAsync(cts.Token);
                responseOutput = await proc.StandardOutput.ReadToEndAsync(cts.Token);
            }
            catch (Exception)
            {
                ok = false;
            }

            return (ok, responseOutput);
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
        /// Create the shell process info for starting process
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
