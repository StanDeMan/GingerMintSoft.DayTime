using SysNet = System.Net; 

namespace GingerMintSoft.DayTimeService.WebApp.Hardware
{
    public static class Platform
    {
        public enum EnmOperatingSystem
        {
            Unknown,
            Windows,
            Linux
        }

        private const string ProgramFile = "/bin/bash";
        private const string ProgramFileWindows = @"\dev\bash";
        public static EnmOperatingSystem OperatingSystem { get; set; }
        public static string? ProgramPath { get; private set; }
        public static string? Dns { get; set; }

        static Platform()
        {
            // If not running on pi set environment for windows platform
            OperatingSystem = Environment.OSVersion.Platform != PlatformID.Win32NT 
                ? RunOnOperatingSystem(EnmOperatingSystem.Linux) 
                : RunOnOperatingSystem(EnmOperatingSystem.Windows);
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
            ProgramPath = Path.Combine(currentPath, path ?? "");
        }

        private static EnmOperatingSystem RunOnOperatingSystem(EnmOperatingSystem os)
        {
            return os ==  EnmOperatingSystem.Linux 
                ? RunOnLinux() 
                : RunOnWindows();
        }

        private static EnmOperatingSystem RunOnWindows()
        {
            ProgramPath = Directory.GetCurrentDirectory() + ProgramFileWindows;
            SetPath(ProgramPath);

            return EnmOperatingSystem.Windows;
        }

        private static EnmOperatingSystem RunOnLinux()
        {
            ProgramPath = ProgramFile;
            Dns = $"{SysNet.Dns.GetHostName()}.local";

            return EnmOperatingSystem.Linux;
        }
    }
}
