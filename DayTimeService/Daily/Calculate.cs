using GingerMintSoft.DayTime;

namespace DayTimeService.Daily
{
    public class Calculate
    {
        /// <summary>
        /// Calculate sun rise and sun set times by geo coordinates
        /// </summary>
        /// <param name="date">Calculate for this specific date</param>
        /// <param name="execute">Object with geo coordinates and a time offset</param>
        /// <returns>Times of sun rise and sun set for this day</returns>
        public static Day SunRiseSunSet(DateTime date, Workload execute)
        {
            var day = new CalcDayTime().SunriseSunset(
                date,
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
        /// <param name="execute">In this object is the offset included</param>
        /// <returns>Times of sun rise and sun set for this day with offset</returns>
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
        /// Else use no offset at all
        /// If it is not in time range the bounds are reset (set to 0)
        /// </summary>
        /// <param name="offset">TimeSpan offset set in DailyWorkload</param>
        /// <returns>Stripped down TimeSpan</returns>
        private static double CalcAndSetBounds(double? offset)
        {
            return offset!.Value >= -120 && offset.Value <= 120 
                ? offset.Value 
                : 0;
        }
    }
}
