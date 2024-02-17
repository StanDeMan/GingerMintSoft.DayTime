using GingerMintSoft.DayTime;

namespace GingerMintSoft.DayTimeTest
{
    [TestClass]
    public class DayTimeTest
    {
        [TestMethod]
        public void TestSunriseSunSetTime()
        {
            var currentDay = new DateTime(2024, 2, 16);

            // Parameters : year - month - day - lat - long
            CalcDayTime.SunriseSunset(currentDay.Year, currentDay.Month, currentDay.Day, 48.10507778308992, 7.90856839921184, out var sunRise, out var sunSet);
            
            var sunRiseTime = TimeSpan.FromHours(sunRise);
            var sunSetTime = TimeSpan.FromHours(sunSet);

            var sunRiseOfActDay = DateTime.Parse(sunRiseTime.ToString(@"hh\:mm\:ss")).ToLocalTime().TimeOfDay;
            var sunSetOfActDay = DateTime.Parse(sunSetTime.ToString(@"hh\:mm\:ss")).ToLocalTime().TimeOfDay;

            Console.WriteLine("Sunrise: " + sunRiseOfActDay);
            Console.WriteLine("Sunset: " + sunSetOfActDay);

            // check sun rise and sun set for day 16.02.2024
            Assert.AreEqual("07:33:51", sunRiseOfActDay.ToString());
            Assert.AreEqual("17:51:00", sunSetOfActDay.ToString());
        }
    }
}