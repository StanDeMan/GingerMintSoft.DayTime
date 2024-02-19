using System.Text;
using DayTimeService.Hardware;

namespace DayTimeService.Execute
{
    public class Command
    {
        private static readonly StreamWriter Writer;
        private static readonly ILogger Logger = new Logger<Command>(Logging.Logger.LoggerFactory);

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
        /// <param name="parameter">Command property</param>
        /// <returns>True: if went ok</returns>
        public static bool Execute(dynamic parameter)
        {
            try
            {
                var cmd = Convert.ToString(parameter.Command);

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
