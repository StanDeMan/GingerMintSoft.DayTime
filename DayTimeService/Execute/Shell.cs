using System.Text;
using DayTimeService.Hardware;

namespace DayTimeService.Execute
{
    public class Shell
    {
        private static readonly StreamWriter Writer;
        private static readonly ILogger<Shell> Logger = LoggerFactory
            .Create(logging => logging.AddConsole())
            .CreateLogger<Shell>();

        /// <summary>
        /// Constructor:
        /// -> On development machine local path is set to: ./dev/pigpio file
        /// -> On linux the path is set to pigpio deamon: /dev/pigpio
        /// </summary>
        static Shell()
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
                Writer.Write(@$"{Convert.ToString(command)}{Environment.NewLine}");
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
