using GingerMintSoft.DayTime;

namespace DayTimeService.Day
{
    public class DayTime
    {
        public void ReadSunTimes()
        {
            var home = new Coordinates()
            {
                Latitude = 48.10507778308992,
                Longitude = 7.90856839921184
            };

            // iterate thru whole year 
            var actDate = DateTime.Now;

            // Parameters : year - month - day - lat - long
            // ReSharper disable once UnusedVariable
            var actDay = new CalcDayTime().SunriseSunset(actDate, home);
        }
     }
}
