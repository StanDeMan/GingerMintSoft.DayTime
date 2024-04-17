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
            var ok = true;

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
    }
}
