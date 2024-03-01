using GingerMintSoft.DayTime;

namespace DayTimeService.Daily
{
    public class Calculate
    {
        public static Day SunRiseSunSet(DateTime actDate, Workload execute)
        {
            var day = new CalcDayTime().SunriseSunset(
                actDate,
                new GingerMintSoft.DayTime.Coordinate()
                {
                    Latitude = execute.Program.Coordinate.Latitude,
                    Longitude = execute.Program.Coordinate.Longitude
                });
            
            day = AppendOffsetTimes(day, execute);

            return day;
        }

        /// <summary>
        /// Take offsets to avoid twilight time regions
        /// </summary>
        /// <param name="day">For this day</param>
        /// <param name="execute"></param>
        /// <returns></returns>
        private static Day AppendOffsetTimes(Day day, Workload execute)
        {
            day.SunRise = day.SunRise.Add(
                TimeSpan.FromMinutes(
                    CalcAndSetBounds(
                        execute.Program.Tasks[Convert.ToInt32(DayTimeServiceWorker.Day.SunRise)]
                            .Offset)));

            day.SunSet = day.SunSet.Add(
                TimeSpan.FromMinutes(
                    CalcAndSetBounds(
                        execute.Program.Tasks[Convert.ToInt32(DayTimeServiceWorker.Day.SunSet)]
                            .Offset)));

            return day;
        }

        /// <summary>
        /// Keep in a time range of +/- 90 minutes
        /// If it is not in time range the bounds are reset (set to 0)
        /// </summary>
        /// <param name="offset">TimeSpan offset set in DailyWorkload</param>
        /// <returns>Stripped down TimeSpan</returns>
        private static double CalcAndSetBounds(double? offset)
        {
            return offset!.Value >= -90 && offset.Value <= 90 
                ? offset.Value 
                : 0;
        }
    }
}
