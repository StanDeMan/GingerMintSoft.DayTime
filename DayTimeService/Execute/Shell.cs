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
        /// Internal execution:
        /// Send to pigpio deamon via cli
        /// </summary>
        /// <param name="command">Command to execute: send via cli</param>
        /// <returns>True: if went ok</returns>
        public static bool Execute(string command)
        {
            try
            {
                Writer.Write(@$"{command}{Environment.NewLine}");
                Writer.Flush();
            }
            catch (Exception e)
            {
                Logger.LogError($"Shell.Execute: {e}");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Internal execution:
        /// Send to pigpio deamon via cli
        /// </summary>
        /// <param name="command">Command to execute: send via cli</param>
        /// <returns>True: if went ok</returns>
        public static async Task<bool> ExecuteAsync(string command)
        {
            try
            {
                await Writer.WriteAsync(@$"{command}{Environment.NewLine}");
                await Writer.FlushAsync();
            }
            catch (Exception e)
            {
                Logger.LogError($"Shell.ExecuteAsync: {e}");

                return false;
            }

            return true;
        }
    }
}
