using System.Text;
using DayTimeService.Hardware;

namespace DayTimeService.Execute
{
    public class Command
    {
        private static readonly StreamWriter Writer;
        private static readonly ILogger<Command> Logger = LoggerFactory
            .Create(logging => logging.AddConsole())
            .CreateLogger<Command>();

        /// <summary>
        /// Constructor:
        /// -> On development machine local path is set to: ./dev/pigpio file
        /// -> On linux the path is set to pigpio deamon: /dev/pigpio
        /// </summary>
        static Command()
        {
            var gpio = Platform.DevicePath;
            var fileStream = new FileInfo(gpio!).OpenWrite();
            Writer = new StreamWriter(fileStream, Encoding.ASCII);
        }

        /// <summary>
        /// Internal execution
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns>True: if went ok</returns>
        public static bool Execute(dynamic command)
        {
            try
            {
                var cmd = Convert.ToString(command);

                Writer.Write(@$"{cmd}{Environment.NewLine}");
                Writer.Flush();
            }
            catch (Exception e)
            {
                Logger.LogError($"Command.Execute (RunInternal): {e}");

                return false;
            }

            return true;
        }
    }
}
