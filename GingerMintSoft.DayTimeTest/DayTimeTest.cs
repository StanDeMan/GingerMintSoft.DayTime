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

        [TestMethod]
        public void TestSunriseSunSetTime()
        {
            // Parameters : year - month - day - lat - long
            CalcDayTime.SunriseSunset(
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
            Assert.AreEqual("07:33:51", sunRiseOfActDay.ToString());
            Assert.AreEqual("17:51:00", sunSetOfActDay.ToString());
        }

        [TestMethod]
        public void TestSunriseSunSetCivilTwilightTime()
        {
            CalcDayTime.CivilTwilight(
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
            Assert.AreEqual("07:01:57", sunRiseOfActDay.ToString());
            Assert.AreEqual("18:22:54", sunSetOfActDay.ToString());
        }

        [TestMethod]
        public void TestSunriseSunSetNauticalTwilightTime()
        {
            CalcDayTime.NauticalTwilight(
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
            Assert.AreEqual("06:25:35", sunRiseOfActDay.ToString());
            Assert.AreEqual("18:59:16", sunSetOfActDay.ToString());
        }

        [TestMethod]
        public void TestSunriseSunSetAstronomicalTwilightTime()
        {
            CalcDayTime.AstronomicalTwilight(
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
            Assert.AreEqual("05:49:36", sunRiseOfActDay.ToString());
            Assert.AreEqual("19:35:15", sunSetOfActDay.ToString());
        }
    }
}