using DayTimeService.Hardware;

namespace DayTimeService.Execute
{
    public sealed class Command
    {
        private static readonly ILogger<Command> Logger = LoggerFactory
            .Create(logging => logging.AddConsole())
            .CreateLogger<Command>();

        /// <summary>
        /// Execute command
        /// Uses bash or gpio folder
        /// Which shell is used is set at workload input sink
        /// </summary>
        /// <param name="command">command to execute</param>
        /// <returns>Returns if executed well</returns>
        public static bool Execute(string command)
        {
            bool ok;

            try
            {
                if (Platform.InputSink == Platform.EnmInputSink.GpioPath)
                {
                    ok = Shell.Execute(command);
                }
                else
                {
                    (ok, _) = Bash.Execute(command);
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Command.Execute (RunInternal): {e}");

                ok = false;
            }

            return ok;
        }

        /// <summary>
        /// Execute command async
        /// Uses bash or gpio folder
        /// Which shell is used is set at workload input sink
        /// </summary>
        /// <param name="command">command to execute</param>
        /// <returns>Returns if executed well</returns>
        public static async Task<bool> ExecuteAsync(string command)
        {
            bool ok;

            try
            {
                if (Platform.InputSink == Platform.EnmInputSink.GpioPath)
                {
                    ok = await Shell.ExecuteAsync(command);
                }
                else
                {
                    (ok, _) = await Bash.ExecuteAsync(command);
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Command.Execute (RunInternal): {e}");

                ok = false;
            }

            return ok;
        }
    }
}
