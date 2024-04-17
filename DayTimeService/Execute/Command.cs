using DayTimeService.Hardware;

namespace DayTimeService.Execute
{
    public sealed class Command
    {
        private static readonly ILogger<Command> Logger = LoggerFactory
            .Create(logging => logging.AddConsole())
            .CreateLogger<Command>();

        public static bool Execute(dynamic command)
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
                    Bash.Execute(command);
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Command.Execute (RunInternal): {e}");

                return false;
            }

            return ok;
        }
    }
}
