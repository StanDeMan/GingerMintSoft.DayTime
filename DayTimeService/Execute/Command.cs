using DayTimeService.Hardware;

namespace DayTimeService.Execute
{
    public sealed class Command
    {
        private static readonly ILogger<Command> Logger = LoggerFactory
            .Create(logging => logging.AddConsole())
            .CreateLogger<Command>();

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
