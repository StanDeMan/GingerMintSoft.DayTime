using SysNet = System.Net; 

namespace DayTimeService.Hardware
{
    public sealed class Platform
    {
        public enum EnmOperatingSystem
        {
            Unknown,
            Windows,
            Linux
        }

        public enum EnmOutputSink
        {
            Unknown,
            Bash,
            GpioPath
        }

        private const string ProgramFile = "/bin/bash";
        private const string GpioFile = "/dev/pigpio";
        private const string ProgramFileWindows = @"\dev\bash";

        public static EnmOperatingSystem OperatingSystem { get; set; }
        public static string? ProgramPath { get; private set; }
        public static string? DevicePath { get; private set; }
        public static string? Dns { get; set; }
        public static EnmOutputSink OutputSink { get; set; }

        static Platform()
        {
            OutputSink = EnmOutputSink.GpioPath;

            // If not running on pi set environment for windows platform
            OperatingSystem = Environment.OSVersion.Platform != PlatformID.Win32NT 
                ? RunOnOperatingSystem(EnmOperatingSystem.Linux) 
                : RunOnOperatingSystem(EnmOperatingSystem.Windows);
        }

        private static EnmOperatingSystem RunOnOperatingSystem(EnmOperatingSystem os)
        {
            return os ==  EnmOperatingSystem.Linux 
                ? RunOnLinux() 
                : RunOnWindows();
        }

        /// <summary>
        /// Rebuild the path to windows convention:
        /// This is for debugging and simulating purpose 
        /// of the gpio if run under windows
        /// </summary>
        /// <param name="path">Linux path convention</param>
        private static void SetPath(string? path)
        {
            var currentPath = Path.GetFullPath(@"..\..\");  
            path = path?.TrimStart('/').Replace('/', '\\');
            DevicePath = Path.Combine(currentPath, path ?? "");
            ProgramPath = Path.Combine(currentPath, path ?? "");
        }

        private static EnmOperatingSystem RunOnWindows()
        {
            var path = OutputSink == EnmOutputSink.GpioPath
                ? DevicePath = Directory.GetCurrentDirectory() + GpioFile
                : ProgramPath = Directory.GetCurrentDirectory() + ProgramFileWindows;

            SetPath(path);

            return EnmOperatingSystem.Windows;
        }

        private static EnmOperatingSystem RunOnLinux()
        {
            DevicePath = GpioFile;
            ProgramPath = ProgramFile;
            Dns = $"{SysNet.Dns.GetHostName()}.local";

            return EnmOperatingSystem.Linux;
        }
    }
}
