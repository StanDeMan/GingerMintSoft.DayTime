using GingerMintSoft.DayTime;

namespace DayTimeService.Day
{
    public class DayTime
    {
        // default home location
        private readonly Coordinate _home = new Coordinate()
        {
            Latitude = 48.10507778308992,
            Longitude = 7.90856839921184
        };

        public GingerMintSoft.DayTime.Day ReadSunTimes(DateTime actDate, Coordinate? coordinate = null)
        {
            coordinate ??= _home;

            return new CalcDayTime().SunriseSunset(actDate, coordinate);
        }
     }
}
