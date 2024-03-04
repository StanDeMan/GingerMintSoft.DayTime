using System.Numerics;
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
            
            day = AppendTimesOffset(day, execute);

            return day;
        }

        /// <summary>
        /// Take offsets to avoid twilight time regions
        /// </summary>
        /// <param name="day">For this day</param>
        /// <param name="execute">In this object is the offset included</param>
        /// <returns>Times of sun rise and sun set for this day with offset</returns>
        private static Day AppendTimesOffset(Day day, Workload execute)
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
        /// Keep in a time range of +/- 120 minutes
        /// Else use no offset at all
        /// If it is not in time range the bounds are reset (set to 0)
        /// </summary>
        /// <param name="offset">TimeSpan offset set in DailyWorkload</param>
        /// <returns>Stripped down TimeSpan</returns>
        private static double CalcAndSetBounds(double offset)
        {
            // take this range from negative to positive value
            const int range = 120;

            return offset is >= -range and <= range 
                ? offset 
                : range * IsPositiveOrNegative(offset);
        }

        /// <summary>
        /// Check sign of value
        /// </summary>
        /// <typeparam name="T">For this type</typeparam>
        /// <param name="number">Check this number</param>
        /// <returns>Negative: -1 v positive: 1 and for zero: 0</returns>
        public static int IsPositiveOrNegative<T>(T number) where T : ISignedNumber<T>
        {
            if (number == T.Zero) return 0;

            return T.IsPositive(number) ? 1 : -1;
        }
    }
}
