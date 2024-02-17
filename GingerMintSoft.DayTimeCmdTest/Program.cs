using GingerMintSoft.DayTime;

namespace GingerMintSoft.DayTimeCmdTest
{
    internal class Program
    {
        static void Main()
        {
            var currentDay = DateTime.Now;

            // Parameters : year - month - day - lat - long
            CalcDayTime.SunriseSunset(currentDay.Year, currentDay.Month, currentDay.Day, 48.10507778308992, 7.90856839921184, out var sunRise, out var sunSet);
            
            var sunRiseTime = TimeSpan.FromHours(sunRise);
            var sunSetTime = TimeSpan.FromHours(sunSet);

            var sunriseTimeString = sunRiseTime.ToString(@"hh\:mm\:ss");
            var sunsetTimeString = sunSetTime.ToString(@"hh\:mm\:ss");
            
            CalcDayTime.CivilTwilight(2017, 2, 6, 46.214973, 5.241947, out sunRise, out sunSet);
            CalcDayTime.NauticalTwilight(2017, 2, 6, 46.214973, 5.241947, out sunRise, out sunSet);
            CalcDayTime.AstronomicalTwilight(2017, 2, 6, 46.214973, 5.241947, out sunRise, out sunSet);

            Console.WriteLine(sunRise + " " + DateTime.Parse(sunriseTimeString).ToLocalTime().TimeOfDay);
            Console.WriteLine(sunSet + " " + DateTime.Parse(sunsetTimeString).ToLocalTime().TimeOfDay);
            Console.ReadKey();
        }
    }
}
