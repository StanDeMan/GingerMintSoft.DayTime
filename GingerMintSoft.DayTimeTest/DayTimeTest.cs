using GingerMintSoft.DayTime;

namespace GingerMintSoft.DayTimeTest
{
    [TestClass]
    public class DayTimeTest
    {
        // test for day 16.02.2024
        public DateTime CurrentDay { get; } = new DateTime(2024, 2, 16);
        public double Latitude { get; set; } = 48.10507778308992;
        public double Longitude { get; set; } = 7.90856839921184;

        // take DST into account
        public int OffsetDst;
        public int HourSunRise = 6;
        public int HourSunSet = 16;

        [TestInitialize]
        public void Initialize()
        {
            OffsetDst = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now).Hours;
            HourSunRise += OffsetDst;
            HourSunSet += OffsetDst;
        }

        [TestMethod]
        public void TestSunriseSunSetTime()
        {
            // Parameters : year - month - day - lat - long
            new CalcDayTime().SunriseSunset(
                CurrentDay.Year, 
                CurrentDay.Month, 
                CurrentDay.Day, 
                Latitude, 
                Longitude, 
                out var sunRise, 
                out var sunSet);
            
            var sunRiseTime = TimeSpan.FromHours(sunRise);
            var sunSetTime = TimeSpan.FromHours(sunSet);

            var sunRiseOfActDay = DateTime.Parse(sunRiseTime.ToString(@"hh\:mm\:ss")).ToLocalTime().TimeOfDay;
            var sunSetOfActDay = DateTime.Parse(sunSetTime.ToString(@"hh\:mm\:ss")).ToLocalTime().TimeOfDay;

            Console.WriteLine("Sunrise: " + sunRiseOfActDay);
            Console.WriteLine("Sunset: " + sunSetOfActDay);

            // check sun rise and sun set for day 16.02.2024
            Assert.AreEqual($"{HourSunRise:00}:33:51", sunRiseOfActDay.ToString());
            Assert.AreEqual($"{HourSunSet:00}:51:00", sunSetOfActDay.ToString());
        }

        [TestMethod]
        public void TestSunriseSunSetCivilTwilightTime()
        {
            new CalcDayTime().CivilTwilight(
                CurrentDay.Year, 
                CurrentDay.Month, 
                CurrentDay.Day, 
                Latitude, 
                Longitude, 
                out var sunRise, 
                out var sunSet);

            var sunRiseTime = TimeSpan.FromHours(sunRise);
            var sunSetTime = TimeSpan.FromHours(sunSet);

            var sunRiseOfActDay = DateTime.Parse(sunRiseTime.ToString(@"hh\:mm\:ss")).ToLocalTime().TimeOfDay;
            var sunSetOfActDay = DateTime.Parse(sunSetTime.ToString(@"hh\:mm\:ss")).ToLocalTime().TimeOfDay;

            Console.WriteLine("Sunrise: " + sunRiseOfActDay);
            Console.WriteLine("Sunset: " + sunSetOfActDay);

            // check sun rise and sun set for day 16.02.2024
            Assert.AreEqual($"{HourSunRise:00}:01:57", sunRiseOfActDay.ToString());
            Assert.AreEqual($"{HourSunSet + 1:00}:22:54", sunSetOfActDay.ToString());
        }

        [TestMethod]
        public void TestSunriseSunSetNauticalTwilightTime()
        {
            new CalcDayTime().NauticalTwilight(
                CurrentDay.Year, 
                CurrentDay.Month, 
                CurrentDay.Day, 
                Latitude, 
                Longitude, 
                out var sunRise, 
                out var sunSet);

            var sunRiseTime = TimeSpan.FromHours(sunRise);
            var sunSetTime = TimeSpan.FromHours(sunSet);

            var sunRiseOfActDay = DateTime.Parse(sunRiseTime.ToString(@"hh\:mm\:ss")).ToLocalTime().TimeOfDay;
            var sunSetOfActDay = DateTime.Parse(sunSetTime.ToString(@"hh\:mm\:ss")).ToLocalTime().TimeOfDay;

            Console.WriteLine("Sunrise: " + sunRiseOfActDay);
            Console.WriteLine("Sunset: " + sunSetOfActDay);

            // check sun rise and sun set for day 16.02.2024
            Assert.AreEqual($"{HourSunRise - 1:00}:25:35", sunRiseOfActDay.ToString());
            Assert.AreEqual($"{HourSunSet + 1:00}:59:16", sunSetOfActDay.ToString());
        }

        [TestMethod]
        public void TestSunriseSunSetAstronomicalTwilightTime()
        {
            new CalcDayTime().AstronomicalTwilight(
                CurrentDay.Year, 
                CurrentDay.Month, 
                CurrentDay.Day, 
                Latitude, 
                Longitude, 
                out var sunRise, 
                out var sunSet);

            var sunRiseTime = TimeSpan.FromHours(sunRise);
            var sunSetTime = TimeSpan.FromHours(sunSet);

            var sunRiseOfActDay = DateTime.Parse(sunRiseTime.ToString(@"hh\:mm\:ss")).ToLocalTime().TimeOfDay;
            var sunSetOfActDay = DateTime.Parse(sunSetTime.ToString(@"hh\:mm\:ss")).ToLocalTime().TimeOfDay;

            Console.WriteLine("Sunrise: " + sunRiseOfActDay);
            Console.WriteLine("Sunset: " + sunSetOfActDay);

            // check sun rise and sun set for day 16.02.2024
            Assert.AreEqual($"{HourSunRise - 2:00}:49:36", sunRiseOfActDay.ToString());
            Assert.AreEqual($"{HourSunSet + 2:00}:35:15", sunSetOfActDay.ToString());
        }
    }
}