namespace DayTimeService.Logging
{
    public static class Logger
    {
        public static ILoggerFactory LoggerFactory {get;}

        static Logger() 
        {
            LoggerFactory = new LoggerFactory();
            LoggerFactory.CreateLogger("Logger");
        }
    }
}
